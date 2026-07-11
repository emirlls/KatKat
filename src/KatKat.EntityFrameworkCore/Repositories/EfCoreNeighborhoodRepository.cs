using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreNeighborhoodRepository : KatKatEfCoreRepository<Neighborhood, int>, INeighborhoodRepository
{
    public EfCoreNeighborhoodRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> NameExistsInDistrictAsync(int districtId, string name, int? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.DistrictId == districtId && x.Name == name && (excludedId == null || x.Id != excludedId));
    }

    public async Task<List<Neighborhood>> GetListByDistrictAsync(int districtId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.DistrictId == districtId).ToListAsync();
    }

    public async Task<bool> ExistsForDistrictAsync(int districtId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.DistrictId == districtId);
    }
}
