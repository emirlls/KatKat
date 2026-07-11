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
using Volo.Abp.Settings;

namespace KatKat.Services;

public class ComplexAppService : KatKatAppService, IComplexAppService
{
    private const int LeaderboardCacheMinutes = 15;

    /// <summary>Placeholder used in a cache key wherever a district/neighborhood filter is null (i.e. "no filter").</summary>
    private const string AllFilterCacheKeyToken = "all";

    private const string NearbyLeaderboardCacheKeyPrefix = "nearby";

    private readonly IComplexRepository _complexRepository;
    private readonly IComplexScoreRepository _complexScoreRepository;
    private readonly ComplexManager _complexManager;
    private readonly IDistributedCache<List<LeaderboardDto>> _leaderboardCache;
    private readonly LocationLookupResolver _locationLookupResolver;

    public ComplexAppService(
        IComplexRepository complexRepository,
        IComplexScoreRepository complexScoreRepository,
        ComplexManager complexManager,
        IDistributedCache<List<LeaderboardDto>> leaderboardCache,
        LocationLookupResolver locationLookupResolver)
    {
        _complexRepository = complexRepository;
        _complexScoreRepository = complexScoreRepository;
        _complexManager = complexManager;
        _leaderboardCache = leaderboardCache;
        _locationLookupResolver = locationLookupResolver;
    }

    public async Task<ComplexDto> GetAsync(Guid id)
    {
        var complex = await _complexRepository.GetAsync(id);
        return await MapToComplexDtoAsync(complex);
    }

    public async Task<ComplexDto> CreateAsync(CreateComplexDto input)
    {
        var complex = await _complexManager.CreateAsync(
            input.Name, input.NeighborhoodId, input.Address, input.Latitude, input.Longitude, input.SubscriptionStartDate);

        await _complexRepository.InsertAsync(complex);

        return await MapToComplexDtoAsync(complex);
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

    public async Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input)
    {
        var complex = await _complexRepository.GetAsync(id);

        complex.ExtendSubscription(input.NewEndDate);

        await _complexRepository.UpdateAsync(complex);

        return await MapToComplexDtoAsync(complex);
    }

    public async Task<List<LeaderboardDto>> GetLeaderboardAsync(
        int? districtId = null, int? neighborhoodId = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        var cacheKey = BuildLeaderboardCacheKey(districtId, neighborhoodId, maxResultCount);

        var cached = await _leaderboardCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var scores = await _complexScoreRepository.GetLeaderboardAsync(districtId, neighborhoodId, maxResultCount);
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
            var entries = await GetLeaderboardAsync(districtId, neighborhoodId: null, maxResultCountPerDistrict);
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
            var entries = await GetLeaderboardAsync(districtId, neighborhoodId, maxResultCountPerNeighborhood);
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
        var dto = ObjectMapper.Map<Complex, ComplexDto>(complex);

        var hierarchies = await _locationLookupResolver.ResolveNeighborhoodHierarchiesAsync(new[] { complex.NeighborhoodId });
        var hierarchy = hierarchies[complex.NeighborhoodId];

        dto.City = hierarchy.City;
        dto.District = hierarchy.District;
        dto.Neighborhood = hierarchy.Neighborhood;

        return dto;
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

    private static string BuildLeaderboardCacheKey(int? districtId, int? neighborhoodId, int maxResultCount)
    {
        return $"{districtId?.ToString() ?? AllFilterCacheKeyToken}:{neighborhoodId?.ToString() ?? AllFilterCacheKeyToken}:{maxResultCount}";
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
