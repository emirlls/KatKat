using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IBuildingAppService : IApplicationService
{
    Task<BuildingDto> GetAsync(Guid id);

    Task<List<BuildingDto>> GetListByComplexAsync(Guid complexId);

    Task<BuildingDto> CreateAsync(CreateBuildingDto input);

    Task<BuildingDto> UpdateAsync(Guid id, UpdateBuildingDto input);

    Task DeleteAsync(Guid id);
}
