using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreComplexRepository : KatKatEfCoreRepository<Complex, Guid>, IComplexRepository
{
    public EfCoreComplexRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.Name == name && (excludedId == null || x.Id != excludedId));
    }

    public async Task<bool> ExistsForNeighborhoodAsync(int neighborhoodId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.NeighborhoodId == neighborhoodId);
    }

    public async Task<List<Complex>> SearchAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Complexes.AsQueryable();

        if (neighborhoodId != null)
        {
            query = query.Where(c => c.NeighborhoodId == neighborhoodId);
        }
        else if (districtId != null)
        {
            var neighborhoodIds = dbContext.Neighborhoods.Where(n => n.DistrictId == districtId).Select(n => n.Id);
            query = query.Where(c => neighborhoodIds.Contains(c.NeighborhoodId));
        }
        else if (cityId != null)
        {
            var districtIds = dbContext.Districts.Where(d => d.CityId == cityId).Select(d => d.Id);
            var neighborhoodIds = dbContext.Neighborhoods.Where(n => districtIds.Contains(n.DistrictId)).Select(n => n.Id);
            query = query.Where(c => neighborhoodIds.Contains(c.NeighborhoodId));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.ToLowerInvariant();
            query = query.Where(c => c.Name.ToLower().Contains(normalizedName));
        }

        return await query.OrderBy(c => c.Name).Take(maxResultCount).ToListAsync();
    }
}
