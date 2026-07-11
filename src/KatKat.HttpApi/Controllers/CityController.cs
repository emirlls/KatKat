using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Permissions;
using KatKat.RateLimiting;
using KatKat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Manages Cities (il) - shared reference data used across every tenant.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/cities")]
[RateLimit]
public class CityController : KatKatController, ICityAppService
{
    private readonly ICityAppService _cityAppService;

    public CityController(ICityAppService cityAppService)
    {
        _cityAppService = cityAppService;
    }

    /// <summary>Gets a City by id.</summary>
    [HttpGet("{id}")]
    public Task<CityDto> GetAsync(int id) => _cityAppService.GetAsync(id);

    /// <summary>Lists all Cities.</summary>
    [HttpGet]
    public Task<List<CityDto>> GetListAsync() => _cityAppService.GetListAsync();

    /// <summary>Creates a new City.</summary>
    [HttpPost]
    [Authorize(KatKatPermissions.Cities.Create)]
    public Task<CityDto> CreateAsync(CreateCityDto input) => _cityAppService.CreateAsync(input);

    /// <summary>Updates a City's name.</summary>
    [HttpPut("{id}")]
    [Authorize(KatKatPermissions.Cities.Update)]
    public Task<CityDto> UpdateAsync(int id, UpdateCityDto input) => _cityAppService.UpdateAsync(id, input);

    /// <summary>Deletes a City, if it has no Districts.</summary>
    [HttpDelete("{id}")]
    [Authorize(KatKatPermissions.Cities.Delete)]
    public Task DeleteAsync(int id) => _cityAppService.DeleteAsync(id);
}
