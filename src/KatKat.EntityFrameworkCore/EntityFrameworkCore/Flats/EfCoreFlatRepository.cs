using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Flats;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Flats;

public class EfCoreFlatRepository : EfCoreRepository<KatKatDbContext, Flat, Guid>, IFlatRepository
{
    public EfCoreFlatRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> FlatNumberExistsInBuildingAsync(Guid buildingId, string flatNumber, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.BuildingId == buildingId &&
            x.FlatNumber == flatNumber &&
            (excludedId == null || x.Id != excludedId));
    }

    public async Task<List<Flat>> GetListByBuildingAsync(Guid buildingId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.BuildingId == buildingId).ToListAsync();
    }
}
