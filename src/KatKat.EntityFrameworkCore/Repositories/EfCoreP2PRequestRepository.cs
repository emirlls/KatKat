using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.P2PRequests;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreP2PRequestRepository : KatKatEfCoreRepository<P2PRequest, Guid>, IP2PRequestRepository
{
    public EfCoreP2PRequestRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<P2PRequest>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ComplexId == complexId && (status == null || x.Status == status))
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync();
    }

    public async Task<int> GetFulfilledCountAsync(Guid complexId, DateTime since)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.CountAsync(x =>
            x.ComplexId == complexId &&
            x.Status == P2PRequestStatus.Fulfilled &&
            x.FulfilledAt != null &&
            x.FulfilledAt >= since);
    }
}
