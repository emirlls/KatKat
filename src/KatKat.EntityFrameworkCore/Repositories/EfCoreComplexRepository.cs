using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreComplexRepository : KatKatEfCoreRepository<Complex, Guid>, IComplexRepository
{
    private readonly IDataFilter _dataFilter;

    public EfCoreComplexRepository(IDbContextProvider<KatKatDbContext> dbContextProvider, IDataFilter dataFilter)
        : base(dbContextProvider)
    {
        _dataFilter = dataFilter;
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.Name == name && (excludedId == null || x.Id != excludedId));
    }

    public async Task<bool> ExistsForNeighborhoodAsync(int neighborhoodId)
    {
        // Neighborhood is shared, non-tenant-scoped reference data - any tenant's Complex may
        // reference it, so this "still in use" guard must look across every tenant, not just the
        // caller's own. Without this, a Manager in Tenant A could delete a Neighborhood that only
        // Tenant B's Complex uses (the tenant-scoped check would see nothing and let it through),
        // leaving Tenant B with a dangling NeighborhoodId.
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AnyAsync(x => x.NeighborhoodId == neighborhoodId);
        }
    }

    public async Task<List<Complex>> SearchAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount)
    {
        var dbContext = await GetDbContextAsync();
        var query = ApplySearchFilters(dbContext, dbContext.Complexes.AsQueryable(), cityId, districtId, neighborhoodId, name);
        return await query.OrderBy(c => c.Name).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<Complex>> SearchAcrossAllTenantsAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var dbContext = await GetDbContextAsync();
            var query = ApplySearchFilters(dbContext, dbContext.Complexes.AsQueryable(), cityId, districtId, neighborhoodId, name);
            return await query.OrderBy(c => c.Name).Take(maxResultCount).ToListAsync();
        }
    }

    private static IQueryable<Complex> ApplySearchFilters(
        KatKatDbContext dbContext, IQueryable<Complex> query,
        int? cityId, int? districtId, int? neighborhoodId, string? name)
    {
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

        return query;
    }
}
