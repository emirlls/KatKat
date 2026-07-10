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

    /// <summary>Rejects the booking if it overlaps an existing Confirmed reservation.</summary>
    Task<ResourceReservationDto> CreateAsync(CreateResourceReservationDto input);

    Task<ResourceReservationDto> CancelAsync(Guid id);
}
