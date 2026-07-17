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

    Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input);

    Task DeleteAsync(Guid id);

    /// <summary>Admin-only: the same update, for any Complex regardless of which Tenant owns it.</summary>
    Task<ComplexDto> UpdateAcrossTenantsAsync(Guid id, UpdateComplexDto input);

    /// <summary>Admin-only: the same delete, for any Complex regardless of which Tenant owns it.</summary>
    Task DeleteAcrossTenantsAsync(Guid id);

    /// <summary>
    /// Admin-only: suspends (isActive=false) or restores (isActive=true) a site's public
    /// visibility (leaderboards/nearby-map), independent of its subscription or any of its own data.
    /// </summary>
    Task<ComplexDto> SetActiveAcrossTenantsAsync(Guid id, bool isActive);

    /// <summary>
    /// Admin-only: a Complex's full structure - its Buildings, each Building's Flats, and each
    /// Flat's residents - for the admin "Tüm Siteler" drill-down view.
    /// </summary>
    Task<AdminSiteDetailDto> GetDetailAcrossTenantsAsync(Guid id);

    Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input);

    /// <summary>
    /// Recalculates every Complex's KatKat Score right now instead of waiting for the nightly
    /// ScoreCalculationWorker - meant for demo/admin use so newly-created activity shows up on
    /// the leaderboard/map immediately.
    /// </summary>
    Task RecalculateScoresAsync();

    /// <summary>
    /// Cross-tenant, aggregated-only ranking (KVKK privacy shield). At least one of cityId/
    /// districtId/neighborhoodId is required - there is deliberately no unfiltered, cross-city
    /// ranking, since comparing sites nationwide isn't a meaningful comparison. All three are
    /// AND-combined: city only = that city's own general ranking, city+district = that district's
    /// own ranking, all three = that neighborhood's own ranking.
    /// </summary>
    Task<List<LeaderboardDto>> GetLeaderboardAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null,
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
