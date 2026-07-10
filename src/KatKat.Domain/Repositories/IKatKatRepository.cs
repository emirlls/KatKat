using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace KatKat.Repositories;

/// <summary>
/// Custom base repository all KatKat repositories extend, so cross-cutting query
/// conventions (e.g. batched lookups) only need to be added in one place.
/// </summary>
public interface IKatKatRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    Task<List<TEntity>> GetListByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
}
