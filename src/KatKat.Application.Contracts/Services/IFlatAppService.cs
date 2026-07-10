using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IFlatAppService : IApplicationService
{
    Task<FlatDto> GetAsync(Guid id);

    Task<List<FlatDto>> GetListByBuildingAsync(Guid buildingId);

    Task<FlatDto> CreateAsync(CreateFlatDto input);
}
