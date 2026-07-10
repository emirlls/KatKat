using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Manages Buildings (blocks) within a Complex.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/buildings")]
public class BuildingController : KatKatController, IBuildingAppService
{
    private readonly IBuildingAppService _buildingAppService;

    public BuildingController(IBuildingAppService buildingAppService)
    {
        _buildingAppService = buildingAppService;
    }

    /// <summary>Gets a Building by id.</summary>
    [HttpGet("{id}")]
    public Task<BuildingDto> GetAsync(Guid id) => _buildingAppService.GetAsync(id);

    /// <summary>Lists all Buildings in a Complex.</summary>
    [HttpGet]
    public Task<List<BuildingDto>> GetListByComplexAsync(Guid complexId) =>
        _buildingAppService.GetListByComplexAsync(complexId);

    /// <summary>Creates a new Building in a Complex.</summary>
    [HttpPost]
    public Task<BuildingDto> CreateAsync(CreateBuildingDto input) => _buildingAppService.CreateAsync(input);
}
