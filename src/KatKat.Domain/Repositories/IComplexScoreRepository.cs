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
    /// leaderboard columns callers are allowed to project into a public DTO. Both filters are
    /// optional and AND-combined: (null, null) = general/overall, (district, null) = that
    /// district's own ranking, (district, neighborhood) = that neighborhood's own ranking.
    /// </summary>
    Task<List<ComplexScore>> GetLeaderboardAsync(int? districtId, int? neighborhoodId, int maxResultCount);

    /// <summary>
    /// Cross-tenant, radius-based leaderboard for the map view: candidates are prefiltered with a
    /// cheap SQL bounding box, then filtered to the exact Haversine distance in-memory - each score
    /// is paired with its computed distance (km) from the query point.
    /// </summary>
    Task<List<(ComplexScore Score, double DistanceKm)>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double radiusKm, int maxResultCount);

    /// <summary>Distinct district ids that have at least one calculated score - powers the all-districts leaderboard.</summary>
    Task<List<int>> GetDistinctDistrictIdsWithScoresAsync();

    /// <summary>
    /// Distinct (DistrictId, NeighborhoodId) pairs that have at least one calculated score - powers
    /// the all-neighborhoods leaderboard. Paired with its district since neighborhood ids alone
    /// don't convey which district-level group they belong to for the response shape.
    /// </summary>
    Task<List<(int DistrictId, int NeighborhoodId)>> GetDistinctNeighborhoodIdsWithScoresAsync();
}
