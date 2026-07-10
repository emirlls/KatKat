using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Shared reservable resources within a Complex - guest parking slots and common areas
/// (çardak/barbekü, oyun odası).
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/resources")]
public class ResourceController : KatKatController, IResourceAppService
{
    private readonly IResourceAppService _resourceAppService;

    public ResourceController(IResourceAppService resourceAppService)
    {
        _resourceAppService = resourceAppService;
    }

    /// <summary>Gets a Resource by id.</summary>
    [HttpGet("{id}")]
    public Task<ResourceDto> GetAsync(Guid id) => _resourceAppService.GetAsync(id);

    /// <summary>Lists reservable Resources in a Complex.</summary>
    [HttpGet]
    public Task<List<ResourceDto>> GetListByComplexAsync(Guid complexId) =>
        _resourceAppService.GetListByComplexAsync(complexId);

    /// <summary>Creates a new reservable Resource.</summary>
    [HttpPost]
    public Task<ResourceDto> CreateAsync(CreateResourceDto input) => _resourceAppService.CreateAsync(input);
}
