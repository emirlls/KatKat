using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using KatKat.Resources;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreResourceReservationRepository : KatKatEfCoreRepository<ResourceReservation, Guid>, IResourceReservationRepository
{
    public EfCoreResourceReservationRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ResourceReservation>> GetListByResourceAsync(Guid resourceId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ResourceId == resourceId)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(Guid resourceId, DateTime startTime, DateTime endTime, Guid? excludeReservationId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.ResourceId == resourceId &&
            x.Status == ReservationStatus.Confirmed &&
            (excludeReservationId == null || x.Id != excludeReservationId) &&
            x.StartTime < endTime &&
            startTime < x.EndTime);
    }
}
