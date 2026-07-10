using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.P2PRequests;

namespace KatKat.Repositories;

public interface IP2PRequestRepository : IKatKatRepository<P2PRequest, Guid>
{
    Task<List<P2PRequest>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null);

    Task<int> GetFulfilledCountAsync(Guid complexId, DateTime since);
}
