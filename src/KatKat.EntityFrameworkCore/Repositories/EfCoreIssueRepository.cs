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

public class EfCoreIssueRepository : KatKatEfCoreRepository<Issue, Guid>, IIssueRepository
{
    public EfCoreIssueRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Issue>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ComplexId == complexId && (status == null || x.Statuses == status))
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync();
    }

    public async Task<decimal?> GetAverageResolutionHoursAsync(Guid complexId, DateTime since)
    {
        var dbSet = await GetDbSetAsync();

        var resolvedPairs = await dbSet
            .Where(x =>
                x.ComplexId == complexId &&
                x.Statuses == IssueStatuses.Resolved &&
                x.ResolvedAt != null &&
                x.ResolvedAt >= since)
            .Select(x => new { x.CreationTime, x.ResolvedAt })
            .ToListAsync();

        if (resolvedPairs.Count == 0)
        {
            return null;
        }

        return (decimal)resolvedPairs.Average(pair => (pair.ResolvedAt!.Value - pair.CreationTime).TotalHours);
    }
}
