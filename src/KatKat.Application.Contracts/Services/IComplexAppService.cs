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

    Task<ComplexDto> CreateAsync(CreateComplexDto input);

    Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input);

    Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input);

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
