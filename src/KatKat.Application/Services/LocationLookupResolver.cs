using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Dtos.Common;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Entities;
using Volo.Abp.DependencyInjection;

namespace KatKat.Services;

/// <summary>
/// Batches Id -> LookupDto resolution for City/District/Neighborhood, so callers never need to
/// issue N+1 queries to hydrate the nested {Id, Name} references on other DTOs.
/// </summary>
public class LocationLookupResolver : ITransientDependency
{
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public LocationLookupResolver(
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        INeighborhoodRepository neighborhoodRepository)
    {
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
        _neighborhoodRepository = neighborhoodRepository;
    }

    public Task<Dictionary<int, LookupDto>> ResolveCitiesAsync(IEnumerable<int> ids) => ResolveAsync(_cityRepository, ids);

    public Task<Dictionary<int, LookupDto>> ResolveDistrictsAsync(IEnumerable<int> ids) => ResolveAsync(_districtRepository, ids);

    public Task<Dictionary<int, LookupDto>> ResolveNeighborhoodsAsync(IEnumerable<int> ids) => ResolveAsync(_neighborhoodRepository, ids);

    /// <summary>
    /// Given Neighborhood ids, resolves the full City+District+Neighborhood {Id,Name} triplet for
    /// each in 3 batched queries total, no N+1.
    /// </summary>
    public async Task<Dictionary<int, (LookupDto City, LookupDto District, LookupDto Neighborhood)>> ResolveNeighborhoodHierarchiesAsync(
        IEnumerable<int> neighborhoodIds)
    {
        var neighborhoods = await _neighborhoodRepository.GetListByIdsAsync(neighborhoodIds.Distinct());
        var districts = await _districtRepository.GetListByIdsAsync(neighborhoods.Select(n => n.DistrictId).Distinct());
        var districtById = districts.ToDictionary(d => d.Id);
        var cities = await _cityRepository.GetListByIdsAsync(districts.Select(d => d.CityId).Distinct());
        var cityById = cities.ToDictionary(c => c.Id);

        return neighborhoods.ToDictionary(n => n.Id, n =>
        {
            var district = districtById[n.DistrictId];
            var city = cityById[district.CityId];
            return (
                City: new LookupDto { Id = city.Id, Name = city.Name },
                District: new LookupDto { Id = district.Id, Name = district.Name },
                Neighborhood: new LookupDto { Id = n.Id, Name = n.Name });
        });
    }

    private static async Task<Dictionary<int, LookupDto>> ResolveAsync<TEntity>(IKatKatRepository<TEntity, int> repository, IEnumerable<int> ids)
        where TEntity : class, IEntity<int>, IHasDisplayName
    {
        var entities = await repository.GetListByIdsAsync(ids.Distinct());
        return entities.ToDictionary(e => e.Id, e => new LookupDto { Id = e.Id, Name = e.Name });
    }
}
