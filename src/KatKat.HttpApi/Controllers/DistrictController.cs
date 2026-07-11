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
/// Manages Districts (ilçe) within a City.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/districts")]
[RateLimit]
public class DistrictController : KatKatController, IDistrictAppService
{
    private readonly IDistrictAppService _districtAppService;

    public DistrictController(IDistrictAppService districtAppService)
    {
        _districtAppService = districtAppService;
    }

    /// <summary>Gets a District by id.</summary>
    [HttpGet("{id}")]
    public Task<DistrictDto> GetAsync(int id) => _districtAppService.GetAsync(id);

    /// <summary>Lists all Districts in a City.</summary>
    [HttpGet]
    public Task<List<DistrictDto>> GetListByCityAsync(int cityId) => _districtAppService.GetListByCityAsync(cityId);

    /// <summary>Creates a new District in a City.</summary>
    [HttpPost]
    [Authorize(KatKatPermissions.Districts.Create)]
    public Task<DistrictDto> CreateAsync(CreateDistrictDto input) => _districtAppService.CreateAsync(input);

    /// <summary>Updates a District's City and/or name.</summary>
    [HttpPut("{id}")]
    [Authorize(KatKatPermissions.Districts.Update)]
    public Task<DistrictDto> UpdateAsync(int id, UpdateDistrictDto input) => _districtAppService.UpdateAsync(id, input);

    /// <summary>Deletes a District, if it has no Neighborhoods.</summary>
    [HttpDelete("{id}")]
    [Authorize(KatKatPermissions.Districts.Delete)]
    public Task DeleteAsync(int id) => _districtAppService.DeleteAsync(id);
}
