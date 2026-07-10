using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IResourceAppService : IApplicationService
{
    Task<ResourceDto> GetAsync(Guid id);

    Task<List<ResourceDto>> GetListByComplexAsync(Guid complexId);

    Task<ResourceDto> CreateAsync(CreateResourceDto input);
}
