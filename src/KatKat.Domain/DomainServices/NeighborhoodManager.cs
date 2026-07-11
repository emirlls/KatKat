using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class NeighborhoodManager : DomainService
{
    private readonly INeighborhoodRepository _neighborhoodRepository;
    private readonly IDistrictRepository _districtRepository;

    public NeighborhoodManager(INeighborhoodRepository neighborhoodRepository, IDistrictRepository districtRepository)
    {
        _neighborhoodRepository = neighborhoodRepository;
        _districtRepository = districtRepository;
    }

    public virtual async Task<Neighborhood> CreateAsync(int districtId, string name)
    {
        await _districtRepository.GetAsync(districtId);

        if (await _neighborhoodRepository.NameExistsInDistrictAsync(districtId, name))
        {
            throw new BusinessException(KatKatErrorCodes.NeighborhoodNameAlreadyExistsInDistrict)
                .WithData("name", name);
        }

        return new Neighborhood(districtId, name);
    }
}
