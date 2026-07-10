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

public class EfCoreComplexScoreRepository : KatKatEfCoreRepository<ComplexScore, Guid>, IComplexScoreRepository
{
    public EfCoreComplexScoreRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<ComplexScore?> FindByComplexIdAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.ComplexId == complexId);
    }

    public async Task<List<ComplexScore>> GetLeaderboardAsync(string? district, int maxResultCount)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => district == null || x.District == district)
            .OrderByDescending(x => x.TotalScore)
            .Take(maxResultCount)
            .ToListAsync();
    }
}
