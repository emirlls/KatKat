using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreDistrictRepository : KatKatEfCoreRepository<District, int>, IDistrictRepository
{
    public EfCoreDistrictRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> NameExistsInCityAsync(int cityId, string name, int? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.CityId == cityId && x.Name == name && (excludedId == null || x.Id != excludedId));
    }

    public async Task<List<District>> GetListByCityAsync(int cityId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.CityId == cityId).ToListAsync();
    }

    public async Task<bool> ExistsForCityAsync(int cityId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.CityId == cityId);
    }
}
