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

    Task<FlatDto> UpdateAsync(Guid id, UpdateFlatDto input);

    Task DeleteAsync(Guid id);

    /// <summary>Flats where the CURRENT user has a FlatMember record, within the given Complex.</summary>
    Task<List<FlatDto>> GetMyFlatsAsync(Guid complexId);
}
