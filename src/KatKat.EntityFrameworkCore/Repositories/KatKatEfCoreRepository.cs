using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

/// <summary>
/// Custom base repository implementation. Every KatKat EF Core repository derives from this
/// instead of the generic EfCoreRepository directly, so shared query logic lives in one place.
/// </summary>
public class KatKatEfCoreRepository<TEntity, TKey> : EfCoreRepository<KatKatDbContext, TEntity, TKey>, IKatKatRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public KatKatEfCoreRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<TEntity>> GetListByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids as ICollection<TKey> ?? ids.ToList();
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => idList.Contains(x.Id)).ToListAsync(cancellationToken);
    }
}
