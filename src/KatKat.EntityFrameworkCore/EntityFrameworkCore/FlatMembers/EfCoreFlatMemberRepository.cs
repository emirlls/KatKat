using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.FlatMembers;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.FlatMembers;

public class EfCoreFlatMemberRepository : EfCoreRepository<KatKatDbContext, FlatMember, Guid>, IFlatMemberRepository
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
}
