using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IComplexScoreRepository : IKatKatRepository<ComplexScore, Guid>
{
    Task<ComplexScore?> FindByComplexIdAsync(Guid complexId);

    /// <summary>
    /// Cross-tenant by design (see ComplexScore remarks) - returns only the aggregated
    /// leaderboard columns callers are allowed to project into a public DTO, excluding any
    /// admin-suspended (IsActive=false) site. All three filters are optional and AND-combined:
    /// (city, null, null) = that city's own general ranking, (city, district, null) = that
    /// district's own ranking, (city, district, neighborhood) = that neighborhood's own ranking.
    /// </summary>
    Task<List<ComplexScore>> GetLeaderboardAsync(int? cityId, int? districtId, int? neighborhoodId, int maxResultCount);

    /// <summary>
    /// Cross-tenant, radius-based leaderboard for the map view: candidates are prefiltered with a
    /// cheap SQL bounding box, then filtered to the exact Haversine distance in-memory - each score
    /// is paired with its computed distance (km) from the query point. Excludes any admin-suspended
    /// (IsActive=false) site.
    /// </summary>
    Task<List<(ComplexScore Score, double DistanceKm)>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double radiusKm, int maxResultCount);

    /// <summary>Distinct district ids with at least one active, calculated score - powers the all-districts leaderboard.</summary>
    Task<List<int>> GetDistinctDistrictIdsWithScoresAsync();

    /// <summary>
    /// Distinct (DistrictId, NeighborhoodId) pairs with at least one active, calculated score -
    /// powers the all-neighborhoods leaderboard. Paired with its district since neighborhood ids
    /// alone don't convey which district-level group they belong to for the response shape.
    /// </summary>
    Task<List<(int DistrictId, int NeighborhoodId)>> GetDistinctNeighborhoodIdsWithScoresAsync();
}
