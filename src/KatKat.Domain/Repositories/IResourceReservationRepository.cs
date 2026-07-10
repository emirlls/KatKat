using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IResourceReservationRepository : IKatKatRepository<ResourceReservation, Guid>
{
    Task<List<ResourceReservation>> GetListByResourceAsync(Guid resourceId);

    Task<bool> HasOverlapAsync(Guid resourceId, DateTime startTime, DateTime endTime, Guid? excludeReservationId = null);
}
