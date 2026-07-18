using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IResourceReservationAppService : IApplicationService
{
    Task<ResourceReservationDto> GetAsync(Guid id);

    Task<List<ResourceReservationDto>> GetListByResourceAsync(Guid resourceId);

    /// <summary>Requests a booking (starts Pending); rejected if it overlaps a Confirmed reservation.</summary>
    Task<ResourceReservationDto> CreateAsync(CreateResourceReservationDto input);

    /// <summary>Manager-only: approves a pending reservation (re-checks overlap), notifying the reserver.</summary>
    Task<ResourceReservationDto> ApproveAsync(Guid id);

    /// <summary>Manager-only: rejects a pending reservation with a reason, notifying the reserver.</summary>
    Task<ResourceReservationDto> RejectAsync(Guid id, RejectResourceReservationDto input);

    Task<ResourceReservationDto> CancelAsync(Guid id);
}
