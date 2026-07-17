using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;

namespace KatKat.Services;

public class CityAppService : KatKatAppService, ICityAppService
{
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly CityManager _cityManager;

    public CityAppService(ICityRepository cityRepository, IDistrictRepository districtRepository, CityManager cityManager)
    {
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
        _cityManager = cityManager;
    }

    public async Task<CityDto> GetAsync(int id)
    {
        var city = await _cityRepository.GetAsync(id);
        return ObjectMapper.Map<City, CityDto>(city);
    }

    public async Task<List<CityDto>> GetListAsync()
    {
        var cities = await _cityRepository.GetListAsync();
        return cities.Select(c => ObjectMapper.Map<City, CityDto>(c)).ToList();
    }

    public async Task<CityDto> CreateAsync(CreateCityDto input)
    {
        var city = await _cityManager.CreateAsync(input.Name);
        await _cityRepository.InsertAsync(city, autoSave: true);
        return ObjectMapper.Map<City, CityDto>(city);
    }

    public async Task<CityDto> UpdateAsync(int id, UpdateCityDto input)
    {
        var city = await _cityRepository.GetAsync(id);
        city.SetName(input.Name);
        await _cityRepository.UpdateAsync(city);
        return ObjectMapper.Map<City, CityDto>(city);
    }

    public async Task DeleteAsync(int id)
    {
        if (await _districtRepository.ExistsForCityAsync(id))
        {
            throw new BusinessException(KatKatErrorCodes.CityHasDistrictsCannotBeDeleted);
        }

        await _cityRepository.DeleteAsync(id);
    }
}
