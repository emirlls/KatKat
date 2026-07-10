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

public class EfCoreUserPreferenceRepository : KatKatEfCoreRepository<UserPreference, Guid>, IUserPreferenceRepository
{
    public EfCoreUserPreferenceRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<UserPreference?> FindByUserIdAsync(Guid userId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<UserPreference>> GetListByUserIdsAsync(IEnumerable<Guid> userIds)
    {
        var idList = userIds as ICollection<Guid> ?? userIds.ToList();
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => idList.Contains(x.UserId)).ToListAsync();
    }
}
