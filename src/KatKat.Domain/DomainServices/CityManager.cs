using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class CityManager : DomainService
{
    private readonly ICityRepository _cityRepository;

    public CityManager(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public virtual async Task<City> CreateAsync(string name)
    {
        if (await _cityRepository.NameExistsAsync(name))
        {
            throw new BusinessException(KatKatErrorCodes.CityNameAlreadyExists)
                .WithData("name", name);
        }

        return new City(name);
    }
}
