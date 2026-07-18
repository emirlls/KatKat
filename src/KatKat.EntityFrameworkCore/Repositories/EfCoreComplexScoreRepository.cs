using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Geo;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreComplexScoreRepository : KatKatEfCoreRepository<ComplexScore, Guid>, IComplexScoreRepository
{
    public EfCoreComplexScoreRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<ComplexScore?> FindByComplexIdAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.ComplexId == complexId);
    }

    public async Task<List<ComplexScore>> GetLeaderboardAsync(int? cityId, int? districtId, int? neighborhoodId, int maxResultCount)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.IsActive &&
                (cityId == null || x.CityId == cityId) &&
                (districtId == null || x.DistrictId == districtId) &&
                (neighborhoodId == null || x.NeighborhoodId == neighborhoodId))
            .OrderByDescending(x => x.TotalScore)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public async Task<List<(ComplexScore Score, double DistanceKm)>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double radiusKm, int maxResultCount)
    {
        var (latitudeDelta, longitudeDelta) = GeoDistanceCalculator.CalculateBoundingBoxDelta(latitude, radiusKm);

        var minLatitude = latitude - latitudeDelta;
        var maxLatitude = latitude + latitudeDelta;
        var minLongitude = longitude - longitudeDelta;
        var maxLongitude = longitude + longitudeDelta;

        // Cheap SQL bounding-box prefilter; the exact Haversine distance check happens in-memory
        // below, since PostGIS-grade geo distance functions are a deliberate non-goal for MVP scale.
        var dbSet = await GetDbSetAsync();
        var candidates = await dbSet
            .Where(x =>
                x.IsActive &&
                x.Latitude >= minLatitude && x.Latitude <= maxLatitude &&
                x.Longitude >= minLongitude && x.Longitude <= maxLongitude)
            .ToListAsync();

        return candidates
            .Select(x => (Score: x, DistanceKm: GeoDistanceCalculator.CalculateDistanceKm(latitude, longitude, x.Latitude, x.Longitude)))
            .Where(x => x.DistanceKm <= radiusKm)
            .OrderByDescending(x => x.Score.TotalScore)
            .Take(maxResultCount)
            .ToList();
    }

    public async Task<List<int>> GetDistinctDistrictIdsWithScoresAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .Select(x => x.DistrictId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<(int DistrictId, int NeighborhoodId)>> GetDistinctNeighborhoodIdsWithScoresAsync()
    {
        var dbSet = await GetDbSetAsync();
        var pairs = await dbSet
            .Where(x => x.IsActive)
            .Select(x => new { x.DistrictId, x.NeighborhoodId })
            .Distinct()
            .ToListAsync();

        return pairs.Select(pair => (pair.DistrictId, pair.NeighborhoodId)).ToList();
    }
}
