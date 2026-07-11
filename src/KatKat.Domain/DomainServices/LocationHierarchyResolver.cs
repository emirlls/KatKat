using System.Threading.Tasks;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

/// <summary>
/// Walks Neighborhood -> District -> City to resolve the full location hierarchy for a
/// Neighborhood id. The single place this join happens on the write side, so callers like
/// ScoreManager don't duplicate it when denormalizing ComplexScore's location FKs.
/// </summary>
public class LocationHierarchyResolver : DomainService
{
    private readonly INeighborhoodRepository _neighborhoodRepository;
    private readonly IDistrictRepository _districtRepository;

    public LocationHierarchyResolver(INeighborhoodRepository neighborhoodRepository, IDistrictRepository districtRepository)
    {
        _neighborhoodRepository = neighborhoodRepository;
        _districtRepository = districtRepository;
    }

    public virtual async Task<(int CityId, int DistrictId, int NeighborhoodId)> ResolveAsync(int neighborhoodId)
    {
        var neighborhood = await _neighborhoodRepository.GetAsync(neighborhoodId);
        var district = await _districtRepository.GetAsync(neighborhood.DistrictId);

        return (district.CityId, district.Id, neighborhood.Id);
    }
}
