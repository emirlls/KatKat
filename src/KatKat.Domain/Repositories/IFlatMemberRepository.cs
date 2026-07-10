using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IFlatMemberRepository : IKatKatRepository<FlatMember, Guid>
{
    Task<bool> ExistsAsync(Guid flatId, Guid userId);

    Task<FlatMember?> FindAsync(Guid flatId, Guid userId);

    Task<List<FlatMember>> GetListByFlatAsync(Guid flatId);

    Task<List<FlatMember>> GetListByUserAsync(Guid userId);

    /// <summary>
    /// Distinct user ids of everyone living in the given Complex (joins Building -> Flat ->
    /// FlatMember), used to resolve who to notify for complex-wide real-time events.
    /// </summary>
    Task<List<Guid>> GetUserIdsByComplexAsync(Guid complexId);
}
