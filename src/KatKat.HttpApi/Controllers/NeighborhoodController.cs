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
/// Manages Neighborhoods (mahalle) within a District.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/neighborhoods")]
[RateLimit]
public class NeighborhoodController : KatKatController, INeighborhoodAppService
{
    private readonly INeighborhoodAppService _neighborhoodAppService;

    public NeighborhoodController(INeighborhoodAppService neighborhoodAppService)
    {
        _neighborhoodAppService = neighborhoodAppService;
    }

    /// <summary>Gets a Neighborhood by id.</summary>
    [HttpGet("{id}")]
    public Task<NeighborhoodDto> GetAsync(int id) => _neighborhoodAppService.GetAsync(id);

    /// <summary>Lists all Neighborhoods in a District.</summary>
    [HttpGet]
    public Task<List<NeighborhoodDto>> GetListByDistrictAsync(int districtId) =>
        _neighborhoodAppService.GetListByDistrictAsync(districtId);

    /// <summary>Creates a new Neighborhood in a District.</summary>
    [HttpPost]
    [Authorize(KatKatPermissions.Neighborhoods.Create)]
    public Task<NeighborhoodDto> CreateAsync(CreateNeighborhoodDto input) => _neighborhoodAppService.CreateAsync(input);

    /// <summary>Updates a Neighborhood's District and/or name.</summary>
    [HttpPut("{id}")]
    [Authorize(KatKatPermissions.Neighborhoods.Update)]
    public Task<NeighborhoodDto> UpdateAsync(int id, UpdateNeighborhoodDto input) =>
        _neighborhoodAppService.UpdateAsync(id, input);

    /// <summary>Deletes a Neighborhood, if no Complex uses it.</summary>
    [HttpDelete("{id}")]
    [Authorize(KatKatPermissions.Neighborhoods.Delete)]
    public Task DeleteAsync(int id) => _neighborhoodAppService.DeleteAsync(id);
}
