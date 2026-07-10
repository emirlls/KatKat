using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace KatKat.FlatMembers;

public interface IFlatMemberRepository : IRepository<FlatMember, Guid>
{
    Task<bool> ExistsAsync(Guid flatId, Guid userId);

    Task<FlatMember?> FindAsync(Guid flatId, Guid userId);

    Task<List<FlatMember>> GetListByFlatAsync(Guid flatId);

    Task<List<FlatMember>> GetListByUserAsync(Guid userId);
}
