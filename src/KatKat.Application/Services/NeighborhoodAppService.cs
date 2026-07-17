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

public class NeighborhoodAppService : KatKatAppService, INeighborhoodAppService
{
    private readonly INeighborhoodRepository _neighborhoodRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly IComplexRepository _complexRepository;
    private readonly NeighborhoodManager _neighborhoodManager;
    private readonly LocationLookupResolver _locationLookupResolver;

    public NeighborhoodAppService(
        INeighborhoodRepository neighborhoodRepository,
        IDistrictRepository districtRepository,
        IComplexRepository complexRepository,
        NeighborhoodManager neighborhoodManager,
        LocationLookupResolver locationLookupResolver)
    {
        _neighborhoodRepository = neighborhoodRepository;
        _districtRepository = districtRepository;
        _complexRepository = complexRepository;
        _neighborhoodManager = neighborhoodManager;
        _locationLookupResolver = locationLookupResolver;
    }

    public async Task<NeighborhoodDto> GetAsync(int id)
    {
        var neighborhood = await _neighborhoodRepository.GetAsync(id);
        return await MapWithDistrictAsync(neighborhood);
    }

    public async Task<List<NeighborhoodDto>> GetListByDistrictAsync(int districtId)
    {
        var neighborhoods = await _neighborhoodRepository.GetListByDistrictAsync(districtId);
        var districts = await _locationLookupResolver.ResolveDistrictsAsync(neighborhoods.Select(n => n.DistrictId));

        return neighborhoods.Select(n =>
        {
            var dto = ObjectMapper.Map<Neighborhood, NeighborhoodDto>(n);
            dto.District = districts[n.DistrictId];
            return dto;
        }).ToList();
    }

    public async Task<NeighborhoodDto> CreateAsync(CreateNeighborhoodDto input)
    {
        var neighborhood = await _neighborhoodManager.CreateAsync(input.DistrictId, input.Name);
        await _neighborhoodRepository.InsertAsync(neighborhood, autoSave: true);
        return await MapWithDistrictAsync(neighborhood);
    }

    public async Task<NeighborhoodDto> UpdateAsync(int id, UpdateNeighborhoodDto input)
    {
        var neighborhood = await _neighborhoodRepository.GetAsync(id);

        await _districtRepository.GetAsync(input.DistrictId);
        neighborhood.SetDistrictId(input.DistrictId);
        neighborhood.SetName(input.Name);

        await _neighborhoodRepository.UpdateAsync(neighborhood);
        return await MapWithDistrictAsync(neighborhood);
    }

    public async Task DeleteAsync(int id)
    {
        if (await _complexRepository.ExistsForNeighborhoodAsync(id))
        {
            throw new BusinessException(KatKatErrorCodes.NeighborhoodInUseByComplexCannotBeDeleted);
        }

        await _neighborhoodRepository.DeleteAsync(id);
    }

    private async Task<NeighborhoodDto> MapWithDistrictAsync(Neighborhood neighborhood)
    {
        var dto = ObjectMapper.Map<Neighborhood, NeighborhoodDto>(neighborhood);
        var districts = await _locationLookupResolver.ResolveDistrictsAsync(new[] { neighborhood.DistrictId });
        dto.District = districts[neighborhood.DistrictId];
        return dto;
    }
}
