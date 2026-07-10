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

public class EfCoreResourceRepository : KatKatEfCoreRepository<Resource, Guid>, IResourceRepository
{
    public EfCoreResourceRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Resource>> GetListByComplexAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.ComplexId == complexId).ToListAsync();
    }
}
