using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class FlatManager : DomainService
{
    private readonly IFlatRepository _flatRepository;
    private readonly IBuildingRepository _buildingRepository;

    public FlatManager(IFlatRepository flatRepository, IBuildingRepository buildingRepository)
    {
        _flatRepository = flatRepository;
        _buildingRepository = buildingRepository;
    }

    public virtual async Task<Flat> CreateAsync(
        Guid buildingId,
        string flatNumber,
        int? floorNumber,
        decimal shareFactor)
    {
        var building = await _buildingRepository.GetAsync(buildingId);

        if (await _flatRepository.FlatNumberExistsInBuildingAsync(buildingId, flatNumber))
        {
            throw new BusinessException(KatKatErrorCodes.FlatNumberAlreadyExistsInBuilding)
                .WithData("flatNumber", flatNumber);
        }

        return new Flat(GuidGenerator.Create(), building.TenantId, buildingId, flatNumber, floorNumber, shareFactor);
    }
}
