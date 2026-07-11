using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IDistrictAppService : IApplicationService
{
    Task<DistrictDto> GetAsync(int id);

    Task<List<DistrictDto>> GetListByCityAsync(int cityId);

    Task<DistrictDto> CreateAsync(CreateDistrictDto input);

    Task<DistrictDto> UpdateAsync(int id, UpdateDistrictDto input);

    Task DeleteAsync(int id);
}
