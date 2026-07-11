using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface INeighborhoodAppService : IApplicationService
{
    Task<NeighborhoodDto> GetAsync(int id);

    Task<List<NeighborhoodDto>> GetListByDistrictAsync(int districtId);

    Task<NeighborhoodDto> CreateAsync(CreateNeighborhoodDto input);

    Task<NeighborhoodDto> UpdateAsync(int id, UpdateNeighborhoodDto input);

    Task DeleteAsync(int id);
}
