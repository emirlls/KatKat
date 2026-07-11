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

public class DistrictAppService : KatKatAppService, IDistrictAppService
{
    private readonly IDistrictRepository _districtRepository;
    private readonly ICityRepository _cityRepository;
    private readonly INeighborhoodRepository _neighborhoodRepository;
    private readonly DistrictManager _districtManager;
    private readonly LocationLookupResolver _locationLookupResolver;

    public DistrictAppService(
        IDistrictRepository districtRepository,
        ICityRepository cityRepository,
        INeighborhoodRepository neighborhoodRepository,
        DistrictManager districtManager,
        LocationLookupResolver locationLookupResolver)
    {
        _districtRepository = districtRepository;
        _cityRepository = cityRepository;
        _neighborhoodRepository = neighborhoodRepository;
        _districtManager = districtManager;
        _locationLookupResolver = locationLookupResolver;
    }

    public async Task<DistrictDto> GetAsync(int id)
    {
        var district = await _districtRepository.GetAsync(id);
        return await MapWithCityAsync(district);
    }

    public async Task<List<DistrictDto>> GetListByCityAsync(int cityId)
    {
        var districts = await _districtRepository.GetListByCityAsync(cityId);
        var cities = await _locationLookupResolver.ResolveCitiesAsync(districts.Select(d => d.CityId));

        return districts.Select(d =>
        {
            var dto = ObjectMapper.Map<District, DistrictDto>(d);
            dto.City = cities[d.CityId];
            return dto;
        }).ToList();
    }

    public async Task<DistrictDto> CreateAsync(CreateDistrictDto input)
    {
        var district = await _districtManager.CreateAsync(input.CityId, input.Name);
        await _districtRepository.InsertAsync(district);
        return await MapWithCityAsync(district);
    }

    public async Task<DistrictDto> UpdateAsync(int id, UpdateDistrictDto input)
    {
        var district = await _districtRepository.GetAsync(id);

        await _cityRepository.GetAsync(input.CityId);
        district.SetCityId(input.CityId);
        district.SetName(input.Name);

        await _districtRepository.UpdateAsync(district);
        return await MapWithCityAsync(district);
    }

    public async Task DeleteAsync(int id)
    {
        if (await _neighborhoodRepository.ExistsForDistrictAsync(id))
        {
            throw new BusinessException(KatKatErrorCodes.DistrictHasNeighborhoodsCannotBeDeleted);
        }

        await _districtRepository.DeleteAsync(id);
    }

    private async Task<DistrictDto> MapWithCityAsync(District district)
    {
        var dto = ObjectMapper.Map<District, DistrictDto>(district);
        var cities = await _locationLookupResolver.ResolveCitiesAsync(new[] { district.CityId });
        dto.City = cities[district.CityId];
        return dto;
    }
}
