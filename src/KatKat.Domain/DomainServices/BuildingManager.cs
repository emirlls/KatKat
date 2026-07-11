using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class BuildingManager : DomainService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IComplexRepository _complexRepository;

    public BuildingManager(IBuildingRepository buildingRepository, IComplexRepository complexRepository)
    {
        _buildingRepository = buildingRepository;
        _complexRepository = complexRepository;
    }

    public virtual async Task<Building> CreateAsync(
        Guid complexId,
        string name,
        int? floorCount = null)
    {
        var complex = await _complexRepository.GetAsync(complexId);

        if (await _buildingRepository.NameExistsInComplexAsync(complexId, name))
        {
            throw new BusinessException(KatKatErrorCodes.BuildingNameAlreadyExistsInComplex)
                .WithData("name", name);
        }

        return new Building(GuidGenerator.Create(), complex.TenantId, complexId, name, floorCount);
    }

    public virtual async Task UpdateAsync(Building building, string name, int? floorCount)
    {
        if (await _buildingRepository.NameExistsInComplexAsync(building.ComplexId, name, building.Id))
        {
            throw new BusinessException(KatKatErrorCodes.BuildingNameAlreadyExistsInComplex)
                .WithData("name", name);
        }

        building.SetName(name);
        building.SetFloorCount(floorCount);
    }
}
