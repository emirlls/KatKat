using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class DistrictManager : DomainService
{
    private readonly IDistrictRepository _districtRepository;
    private readonly ICityRepository _cityRepository;

    public DistrictManager(IDistrictRepository districtRepository, ICityRepository cityRepository)
    {
        _districtRepository = districtRepository;
        _cityRepository = cityRepository;
    }

    public virtual async Task<District> CreateAsync(int cityId, string name)
    {
        await _cityRepository.GetAsync(cityId);

        if (await _districtRepository.NameExistsInCityAsync(cityId, name))
        {
            throw new BusinessException(KatKatErrorCodes.DistrictNameAlreadyExistsInCity)
                .WithData("name", name);
        }

        return new District(cityId, name);
    }
}
