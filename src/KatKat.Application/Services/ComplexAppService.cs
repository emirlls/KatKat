using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Repositories;
using KatKat.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Identity;
using Volo.Abp.Settings;

namespace KatKat.Services;

public class ComplexAppService : KatKatAppService, IComplexAppService
{
    /// <summary>
    /// Kept short (rather than a longer production-style TTL) so a manual score recalculation
    /// shows up in the leaderboard/map almost immediately - this app has no fine-grained cache
    /// invalidation for every possible district/neighborhood/coordinate cache key.
    /// </summary>
    private const int LeaderboardCacheMinutes = 2;

    /// <summary>Placeholder used in a cache key wherever a district/neighborhood filter is null (i.e. "no filter").</summary>
    private const string AllFilterCacheKeyToken = "all";

    private const string NearbyLeaderboardCacheKeyPrefix = "nearby";

    private readonly IComplexRepository _complexRepository;
    private readonly IComplexScoreRepository _complexScoreRepository;
    private readonly ScoreManager _scoreManager;
    private readonly IDistributedCache<List<LeaderboardDto>> _leaderboardCache;
    private readonly LocationLookupResolver _locationLookupResolver;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly IIdentityUserRepository _identityUserRepository;

    public ComplexAppService(
        IComplexRepository complexRepository,
        IComplexScoreRepository complexScoreRepository,
        ScoreManager scoreManager,
        IDistributedCache<List<LeaderboardDto>> leaderboardCache,
        LocationLookupResolver locationLookupResolver,
        IBuildingRepository buildingRepository,
        IFlatRepository flatRepository,
        IFlatMemberRepository flatMemberRepository,
        IIdentityUserRepository identityUserRepository)
    {
        _complexRepository = complexRepository;
        _complexScoreRepository = complexScoreRepository;
        _scoreManager = scoreManager;
        _leaderboardCache = leaderboardCache;
        _locationLookupResolver = locationLookupResolver;
        _buildingRepository = buildingRepository;
        _flatRepository = flatRepository;
        _flatMemberRepository = flatMemberRepository;
        _identityUserRepository = identityUserRepository;
    }

    public async Task<ComplexDto> GetAsync(Guid id)
    {
        var complex = await _complexRepository.GetAsync(id);
        return await MapToComplexDtoAsync(complex);
    }

    public async Task<ComplexDto?> GetMyComplexAsync()
    {
        // GetListAsync is transparently filtered to the current Tenant by ABP's IMultiTenant
        // query filter, so this only ever returns the caller's own site - a Manager/Resident
        // never picks a Complex, they just have (at most) one.
        var complexes = await _complexRepository.GetListAsync();
        var complex = complexes.FirstOrDefault();

        return complex == null ? null : await MapToComplexDtoAsync(complex);
    }

    public async Task<List<ComplexDto>> SearchAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount)
    {
        var complexes = await _complexRepository.SearchAsync(cityId, districtId, neighborhoodId, name, maxResultCount);
        return await MapToComplexDtosAsync(complexes);
    }

    public async Task<List<AdminComplexListItemDto>> SearchAcrossAllTenantsAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount)
    {
        var complexes = await _complexRepository.SearchAcrossAllTenantsAsync(cityId, districtId, neighborhoodId, name, maxResultCount);
        var complexDtos = await MapToComplexDtosAsync(complexes);

        return complexes.Zip(complexDtos, (complex, dto) => new AdminComplexListItemDto
        {
            Complex = dto,
            TenantId = complex.TenantId,
        }).ToList();
    }

    public async Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input)
    {
        var complex = await _complexRepository.GetAsync(id);

        complex.SetName(input.Name);
        complex.SetNeighborhood(input.NeighborhoodId);
        complex.SetAddress(input.Address);
        complex.SetLocation(input.Latitude, input.Longitude);

        await _complexRepository.UpdateAsync(complex);

        return await MapToComplexDtoAsync(complex);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _complexRepository.DeleteAsync(id);
    }

    public async Task<ComplexDto> UpdateAcrossTenantsAsync(Guid id, UpdateComplexDto input)
    {
        var complex = await _complexRepository.GetAcrossAllTenantsAsync(id);

        using (CurrentTenant.Change(complex.TenantId))
        {
            complex.SetName(input.Name);
            complex.SetNeighborhood(input.NeighborhoodId);
            complex.SetAddress(input.Address);
            complex.SetLocation(input.Latitude, input.Longitude);

            await _complexRepository.UpdateAsync(complex);

            return await MapToComplexDtoAsync(complex);
        }
    }

    public async Task DeleteAcrossTenantsAsync(Guid id)
    {
        var complex = await _complexRepository.GetAcrossAllTenantsAsync(id);

        using (CurrentTenant.Change(complex.TenantId))
        {
            await _complexRepository.DeleteAsync(id);
        }
    }

    public async Task<ComplexDto> SetActiveAcrossTenantsAsync(Guid id, bool isActive)
    {
        var complex = await _complexRepository.GetAcrossAllTenantsAsync(id);

        using (CurrentTenant.Change(complex.TenantId))
        {
            if (isActive)
            {
                complex.Activate();
            }
            else
            {
                complex.Deactivate();
            }

            await _complexRepository.UpdateAsync(complex);

            // Reflected immediately, rather than waiting for the next scheduled score
            // recalculation to notice the Complex changed - a suspended site must disappear from
            // the leaderboard/nearby-map right away.
            var complexScore = await _complexScoreRepository.FindByComplexIdAsync(complex.Id);
            if (complexScore != null)
            {
                complexScore.SetActive(isActive);
                await _complexScoreRepository.UpdateAsync(complexScore);
            }

            return await MapToComplexDtoAsync(complex);
        }
    }

    public async Task<AdminSiteDetailDto> GetDetailAcrossTenantsAsync(Guid id)
    {
        var complex = await _complexRepository.GetAcrossAllTenantsAsync(id);

        using (CurrentTenant.Change(complex.TenantId))
        {
            var complexDto = await MapToComplexDtoAsync(complex);

            // Three batched queries (buildings, flats, flat members) regardless of how many
            // buildings/flats the site has, instead of one query per building plus one per flat.
            var buildings = await _buildingRepository.GetListByComplexAsync(id);
            var flatsByBuildingId = (await _flatRepository.GetListByComplexAsync(id))
                .GroupBy(f => f.BuildingId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var membersByFlatId = (await _flatMemberRepository.GetListByComplexAsync(id))
                .GroupBy(m => m.FlatId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // One batched lookup for every resident's username across the whole site, instead of
            // one per flat.
            var allUserIds = membersByFlatId.Values.SelectMany(m => m).Select(m => m.UserId).Distinct();
            var users = await _identityUserRepository.GetListByIdsAsync(allUserIds);
            var userNameById = users.ToDictionary(u => u.Id, u => u.UserName);

            return new AdminSiteDetailDto
            {
                Complex = complexDto,
                TenantId = complex.TenantId,
                Buildings = buildings.Select(building => new AdminBuildingDetailDto
                {
                    Id = building.Id,
                    Name = building.Name,
                    FloorCount = building.FloorCount,
                    Flats = flatsByBuildingId.GetValueOrDefault(building.Id, new List<Flat>()).Select(flat => new AdminFlatDetailDto
                    {
                        Id = flat.Id,
                        FlatNumber = flat.FlatNumber,
                        FloorNumber = flat.FloorNumber,
                        ShareFactor = flat.ShareFactor,
                        Residents = membersByFlatId.GetValueOrDefault(flat.Id, new List<FlatMember>()).Select(member => new AdminResidentDto
                        {
                            Id = member.Id,
                            UserName = userNameById.GetValueOrDefault(member.UserId, member.UserId.ToString()),
                            Role = member.Role,
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
            };
        }
    }

    public async Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input)
    {
        var complex = await _complexRepository.GetAsync(id);

        complex.ExtendSubscription(input.NewEndDate);

        await _complexRepository.UpdateAsync(complex);

        return await MapToComplexDtoAsync(complex);
    }

    public async Task RecalculateScoresAsync()
    {
        var complexes = await _complexRepository.GetListAsync();
        var since = DateTime.UtcNow.AddDays(-ScoreManager.TrailingWindowDays);

        foreach (var complex in complexes)
        {
            await _scoreManager.RecalculateAsync(complex, since);
        }

        // Every per-city/per-district/per-neighborhood permutation relies on the short TTL above -
        // there is no longer a single no-filter "Genel" key to invalidate deterministically, since a
        // city (or narrower) scope is now mandatory.
    }

    /// <summary>
    /// The leaderboard is always scoped to at least a city - there is deliberately no unfiltered,
    /// cross-city "general" ranking, since comparing sites nationwide isn't a meaningful comparison.
    /// </summary>
    public async Task<List<LeaderboardDto>> GetLeaderboardAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        if (cityId == null && districtId == null && neighborhoodId == null)
        {
            throw new BusinessException(KatKatErrorCodes.LeaderboardScopeRequired);
        }

        var cacheKey = BuildLeaderboardCacheKey(cityId, districtId, neighborhoodId, maxResultCount);

        var cached = await _leaderboardCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var scores = await _complexScoreRepository.GetLeaderboardAsync(cityId, districtId, neighborhoodId, maxResultCount);
        var result = await BuildLeaderboardAsync(scores.Select(score => (score, (double?)null)).ToList());

        await CacheLeaderboardAsync(cacheKey, result);

        return result;
    }

    public async Task<List<LeaderboardDto>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double? radiusKm = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        var effectiveRadiusKm = radiusKm ?? await SettingProvider.GetAsync(
            KatKatSettings.NearbyLeaderboardRadiusKm, KatKatConsts.DefaultNearbyLeaderboardRadiusKm);

        if (effectiveRadiusKm <= 0)
        {
            throw new BusinessException(KatKatErrorCodes.NearbyLeaderboardRadiusMustBePositive);
        }

        var cacheKey = BuildNearbyLeaderboardCacheKey(latitude, longitude, effectiveRadiusKm, maxResultCount);

        var cached = await _leaderboardCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var nearbyScores = await _complexScoreRepository.GetNearbyLeaderboardAsync(
            latitude, longitude, effectiveRadiusKm, maxResultCount);

        var result = await BuildLeaderboardAsync(nearbyScores.Select(entry => (entry.Score, (double?)entry.DistanceKm)).ToList());

        await CacheLeaderboardAsync(cacheKey, result);

        return result;
    }

    public async Task<List<DistrictLeaderboardDto>> GetAllDistrictLeaderboardsAsync(
        int maxResultCountPerDistrict = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        var districtIds = await _complexScoreRepository.GetDistinctDistrictIdsWithScoresAsync();

        var result = new List<DistrictLeaderboardDto>(districtIds.Count);
        foreach (var districtId in districtIds)
        {
            var entries = await GetLeaderboardAsync(cityId: null, districtId, neighborhoodId: null, maxResultCountPerDistrict);
            result.Add(new DistrictLeaderboardDto
            {
                District = entries[0].District,
                Entries = entries
            });
        }

        return result;
    }

    public async Task<List<NeighborhoodLeaderboardDto>> GetAllNeighborhoodLeaderboardsAsync(
        int maxResultCountPerNeighborhood = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        var neighborhoodPairs = await _complexScoreRepository.GetDistinctNeighborhoodIdsWithScoresAsync();

        var result = new List<NeighborhoodLeaderboardDto>(neighborhoodPairs.Count);
        foreach (var (districtId, neighborhoodId) in neighborhoodPairs)
        {
            var entries = await GetLeaderboardAsync(cityId: null, districtId, neighborhoodId, maxResultCountPerNeighborhood);
            result.Add(new NeighborhoodLeaderboardDto
            {
                District = entries[0].District,
                Neighborhood = entries[0].Neighborhood,
                Entries = entries
            });
        }

        return result;
    }

    private async Task<ComplexDto> MapToComplexDtoAsync(Complex complex)
    {
        return (await MapToComplexDtosAsync(new List<Complex> { complex }))[0];
    }

    /// <summary>Batches the City/District/Neighborhood hierarchy lookup for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<ComplexDto>> MapToComplexDtosAsync(List<Complex> complexes)
    {
        if (complexes.Count == 0)
        {
            return new List<ComplexDto>();
        }

        var hierarchies = await _locationLookupResolver.ResolveNeighborhoodHierarchiesAsync(
            complexes.Select(c => c.NeighborhoodId));

        return complexes.Select(complex =>
        {
            var dto = ObjectMapper.Map<Complex, ComplexDto>(complex);
            var hierarchy = hierarchies[complex.NeighborhoodId];
            dto.City = hierarchy.City;
            dto.District = hierarchy.District;
            dto.Neighborhood = hierarchy.Neighborhood;
            return dto;
        }).ToList();
    }

    /// <summary>Single place that turns a ranked score list into ranked (Rank starts at 1) DTOs, batch-resolving City/District/Neighborhood names so no leaderboard call does N+1 lookups.</summary>
    private async Task<List<LeaderboardDto>> BuildLeaderboardAsync(IReadOnlyList<(ComplexScore Score, double? DistanceKm)> entries)
    {
        if (entries.Count == 0)
        {
            return new List<LeaderboardDto>();
        }

        var cityLookup = await _locationLookupResolver.ResolveCitiesAsync(entries.Select(e => e.Score.CityId));
        var districtLookup = await _locationLookupResolver.ResolveDistrictsAsync(entries.Select(e => e.Score.DistrictId));
        var neighborhoodLookup = await _locationLookupResolver.ResolveNeighborhoodsAsync(entries.Select(e => e.Score.NeighborhoodId));

        return entries
            .Select((entry, index) => new LeaderboardDto
            {
                Rank = index + 1,
                ComplexId = entry.Score.ComplexId,
                ComplexName = entry.Score.Name,
                City = cityLookup[entry.Score.CityId],
                District = districtLookup[entry.Score.DistrictId],
                Neighborhood = neighborhoodLookup[entry.Score.NeighborhoodId],
                Latitude = entry.Score.Latitude,
                Longitude = entry.Score.Longitude,
                Score = entry.Score.TotalScore,
                CalculatedAt = entry.Score.CalculatedAt,
                DistanceKm = entry.DistanceKm
            })
            .ToList();
    }

    private async Task CacheLeaderboardAsync(string cacheKey, List<LeaderboardDto> result)
    {
        await _leaderboardCache.SetAsync(cacheKey, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LeaderboardCacheMinutes)
        });
    }

    private static string BuildLeaderboardCacheKey(int? cityId, int? districtId, int? neighborhoodId, int maxResultCount)
    {
        return $"{cityId?.ToString() ?? AllFilterCacheKeyToken}:{districtId?.ToString() ?? AllFilterCacheKeyToken}:{neighborhoodId?.ToString() ?? AllFilterCacheKeyToken}:{maxResultCount}";
    }

    private static string BuildNearbyLeaderboardCacheKey(
        decimal latitude, decimal longitude, double radiusKm, int maxResultCount)
    {
        var roundedLatitude = Math.Round(latitude, KatKatConsts.NearbyLeaderboardCacheKeyCoordinateRoundingDecimals);
        var roundedLongitude = Math.Round(longitude, KatKatConsts.NearbyLeaderboardCacheKeyCoordinateRoundingDecimals);

        return FormattableString.Invariant(
            $"{NearbyLeaderboardCacheKeyPrefix}:{roundedLatitude}:{roundedLongitude}:{radiusKm}:{maxResultCount}");
    }
}
