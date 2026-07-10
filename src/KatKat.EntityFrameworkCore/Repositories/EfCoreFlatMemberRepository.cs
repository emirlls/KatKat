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

public class EfCoreFlatMemberRepository : KatKatEfCoreRepository<FlatMember, Guid>, IFlatMemberRepository
{
    public EfCoreFlatMemberRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> ExistsAsync(Guid flatId, Guid userId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.FlatId == flatId && x.UserId == userId);
    }

    public async Task<FlatMember?> FindAsync(Guid flatId, Guid userId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.FlatId == flatId && x.UserId == userId);
    }

    public async Task<List<FlatMember>> GetListByFlatAsync(Guid flatId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.FlatId == flatId).ToListAsync();
    }

    public async Task<List<FlatMember>> GetListByUserAsync(Guid userId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<List<Guid>> GetUserIdsByComplexAsync(Guid complexId)
    {
        var dbContext = await GetDbContextAsync();
        return await (
            from flatMember in dbContext.FlatMembers
            join flat in dbContext.Flats on flatMember.FlatId equals flat.Id
            join building in dbContext.Buildings on flat.BuildingId equals building.Id
            where building.ComplexId == complexId
            select flatMember.UserId
        ).Distinct().ToListAsync();
    }
}
