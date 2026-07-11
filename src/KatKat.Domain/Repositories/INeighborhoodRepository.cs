using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface INeighborhoodRepository : IKatKatRepository<Neighborhood, int>
{
    Task<bool> NameExistsInDistrictAsync(int districtId, string name, int? excludedId = null);

    Task<List<Neighborhood>> GetListByDistrictAsync(int districtId);

    Task<bool> ExistsForDistrictAsync(int districtId);
}
