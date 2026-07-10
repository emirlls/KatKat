using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Buildings;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Buildings;

public class EfCoreBuildingRepository : EfCoreRepository<KatKatDbContext, Building, Guid>, IBuildingRepository
{
    public EfCoreBuildingRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> NameExistsInComplexAsync(Guid complexId, string name, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.ComplexId == complexId &&
            x.Name == name &&
            (excludedId == null || x.Id != excludedId));
    }

    public async Task<List<Building>> GetListByComplexAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.ComplexId == complexId).ToListAsync();
    }
}
