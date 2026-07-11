using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IDistrictRepository : IKatKatRepository<District, int>
{
    Task<bool> NameExistsInCityAsync(int cityId, string name, int? excludedId = null);

    Task<List<District>> GetListByCityAsync(int cityId);

    Task<bool> ExistsForCityAsync(int cityId);
}
