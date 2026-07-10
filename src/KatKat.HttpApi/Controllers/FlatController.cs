using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Manages Flats (apartments) within a Building.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/flats")]
public class FlatController : KatKatController, IFlatAppService
{
    private readonly IFlatAppService _flatAppService;

    public FlatController(IFlatAppService flatAppService)
    {
        _flatAppService = flatAppService;
    }

    /// <summary>Gets a Flat by id.</summary>
    [HttpGet("{id}")]
    public Task<FlatDto> GetAsync(Guid id) => _flatAppService.GetAsync(id);

    /// <summary>Lists all Flats in a Building.</summary>
    [HttpGet]
    public Task<List<FlatDto>> GetListByBuildingAsync(Guid buildingId) =>
        _flatAppService.GetListByBuildingAsync(buildingId);

    /// <summary>Creates a new Flat in a Building.</summary>
    [HttpPost]
    public Task<FlatDto> CreateAsync(CreateFlatDto input) => _flatAppService.CreateAsync(input);
}
