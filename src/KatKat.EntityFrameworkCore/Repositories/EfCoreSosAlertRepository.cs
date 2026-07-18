using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Enums;
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
        var alerts = await dbSet
            .Where(x => x.ComplexId == complexId)
            // A resolved HelpNeeded alert is done - once help has arrived it must stop showing on
            // the floor plan entirely, not linger as a stale "help arrived" row. Excluding it here
            // (before picking the latest per flat) means that flat's status either falls back to
            // an earlier still-relevant report (e.g. its last SafeConfirmed) or simply has no row
            // at all if this was its only report ever.
            .Where(x => x.Status != SosStatuses.HelpNeeded || x.ResolvedAt == null)
            .ToListAsync();

        return alerts
            .GroupBy(x => x.FlatId)
            .Select(g => g.OrderByDescending(x => x.CreationTime).First())
            .ToList();
    }
}
