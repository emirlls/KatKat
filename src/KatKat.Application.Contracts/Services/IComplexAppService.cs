using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IComplexAppService : IApplicationService
{
    Task<ComplexDto> GetAsync(Guid id);

    /// <summary>
    /// The current user's own Complex (their Tenant owns exactly one, or none yet if a Manager
    /// hasn't created their site) - since multi-tenancy means a Manager/Resident only ever
    /// belongs to one site, there is nothing to "pick" here, just what's already theirs.
    /// </summary>
    Task<ComplexDto?> GetMyComplexAsync();

    /// <summary>
    /// Finds Complexes by an optional City/District/Neighborhood filter and/or a case-insensitive
    /// name search - lets the frontend offer a real picker instead of requiring the caller to
    /// already know a Complex's id.
    /// </summary>
    Task<List<ComplexDto>> SearchAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount);

    /// <summary>
    /// Admin-only: the same search, across every Tenant's Complexes at once - lets the platform
    /// admin browse/find any site regardless of which Manager owns it.
    /// </summary>
    Task<List<AdminComplexListItemDto>> SearchAcrossAllTenantsAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount);

    Task<ComplexDto> CreateAsync(CreateComplexDto input);

    Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input);

    Task DeleteAsync(Guid id);

    Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input);

    /// <summary>
    /// Recalculates every Complex's KatKat Score right now instead of waiting for the nightly
    /// ScoreCalculationWorker - meant for demo/admin use so newly-created activity shows up on
    /// the leaderboard/map immediately.
    /// </summary>
    Task RecalculateScoresAsync();

    /// <summary>
    /// Cross-tenant, aggregated-only ranking (KVKK privacy shield). Both filters are optional and
    /// AND-combined: neither given = general/overall, district only = that district's own
    /// ranking, both given = that neighborhood's own ranking within the district.
    /// </summary>
    Task<List<LeaderboardDto>> GetLeaderboardAsync(
        int? districtId = null, int? neighborhoodId = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount);

    /// <summary>
    /// Nearby buildings' KatKat Scores within a radius (km) of the given coordinate, for map
    /// rendering - the radius defaults to the admin-configurable NearbyLeaderboardRadiusKm setting.
    /// </summary>
    Task<List<LeaderboardDto>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double? radiusKm = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount);

    /// <summary>Every district's own leaderboard (each re-ranked from 1), grouped together in one response.</summary>
    Task<List<DistrictLeaderboardDto>> GetAllDistrictLeaderboardsAsync(
        int maxResultCountPerDistrict = KatKatConsts.DefaultLeaderboardMaxResultCount);

    /// <summary>Every neighborhood's own leaderboard (each re-ranked from 1), grouped together in one response.</summary>
    Task<List<NeighborhoodLeaderboardDto>> GetAllNeighborhoodLeaderboardsAsync(
        int maxResultCountPerNeighborhood = KatKatConsts.DefaultLeaderboardMaxResultCount);
}
