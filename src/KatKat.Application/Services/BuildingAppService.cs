using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Volo.Abp;

namespace KatKat.Services;

public class BuildingAppService : KatKatAppService, IBuildingAppService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly BuildingManager _buildingManager;
    private readonly IFlatRepository _flatRepository;

    public BuildingAppService(IBuildingRepository buildingRepository, BuildingManager buildingManager, IFlatRepository flatRepository)
    {
        _buildingRepository = buildingRepository;
        _buildingManager = buildingManager;
        _flatRepository = flatRepository;
    }

    public async Task<BuildingDto> GetAsync(Guid id)
    {
        var building = await _buildingRepository.GetAsync(id);
        return ObjectMapper.Map<Building, BuildingDto>(building);
    }

    public async Task<List<BuildingDto>> GetListByComplexAsync(Guid complexId)
    {
        var buildings = await _buildingRepository.GetListByComplexAsync(complexId);
        return buildings.Select(b => ObjectMapper.Map<Building, BuildingDto>(b)).ToList();
    }

    public async Task<BuildingDto> CreateAsync(CreateBuildingDto input)
    {
        var building = await _buildingManager.CreateAsync(input.ComplexId, input.Name, input.FloorCount);

        await _buildingRepository.InsertAsync(building, autoSave: true);

        return ObjectMapper.Map<Building, BuildingDto>(building);
    }

    public async Task<BuildingDto> UpdateAsync(Guid id, UpdateBuildingDto input)
    {
        var building = await _buildingRepository.GetAsync(id);

        await _buildingManager.UpdateAsync(building, input.Name, input.FloorCount);

        await _buildingRepository.UpdateAsync(building, autoSave: true);

        return ObjectMapper.Map<Building, BuildingDto>(building);
    }

    public async Task DeleteAsync(Guid id)
    {
        var flats = await _flatRepository.GetListByBuildingAsync(id);
        if (flats.Count > 0)
        {
            throw new BusinessException(KatKatErrorCodes.BuildingHasFlatsCannotBeDeleted);
        }

        await _buildingRepository.DeleteAsync(id);
    }
}
