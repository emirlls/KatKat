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

public class EfCoreSosAlertRepository : KatKatEfCoreRepository<SosAlert, Guid>, ISosAlertRepository
{
    public EfCoreSosAlertRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<SosAlert>> GetLatestStatusByComplexAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        var alerts = await dbSet.Where(x => x.ComplexId == complexId).ToListAsync();

        return alerts
            .GroupBy(x => x.FlatId)
            .Select(g => g.OrderByDescending(x => x.CreationTime).First())
            .ToList();
    }
}
