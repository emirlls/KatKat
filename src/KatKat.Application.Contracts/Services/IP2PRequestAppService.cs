using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.P2PRequests;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IP2PRequestAppService : IApplicationService
{
    Task<P2PRequestDto> GetAsync(Guid id);

    Task<List<P2PRequestDto>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null);

    Task<P2PRequestDto> CreateAsync(CreateP2PRequestDto input);

    Task<P2PRequestDto> FulfillAsync(Guid id);

    Task<P2PRequestDto> CancelAsync(Guid id);
}
