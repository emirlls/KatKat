using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface ICityAppService : IApplicationService
{
    Task<CityDto> GetAsync(int id);

    Task<List<CityDto>> GetListAsync();

    Task<CityDto> CreateAsync(CreateCityDto input);

    Task<CityDto> UpdateAsync(int id, UpdateCityDto input);

    Task DeleteAsync(int id);
}
