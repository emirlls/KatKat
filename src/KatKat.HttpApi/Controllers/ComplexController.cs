using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Manages Complexes (apartment sites) - the root tenant entity of KatKat.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/complexes")]
public class ComplexController : KatKatController, IComplexAppService
{
    private readonly IComplexAppService _complexAppService;

    public ComplexController(IComplexAppService complexAppService)
    {
        _complexAppService = complexAppService;
    }

    /// <summary>Gets a Complex by id.</summary>
    [HttpGet("{id}")]
    public Task<ComplexDto> GetAsync(Guid id) => _complexAppService.GetAsync(id);

    /// <summary>Creates a new Complex for the current tenant.</summary>
    [HttpPost]
    public Task<ComplexDto> CreateAsync(CreateComplexDto input) => _complexAppService.CreateAsync(input);

    /// <summary>Updates a Complex's basic info (name, city, district, address).</summary>
    [HttpPut("{id}")]
    public Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input) => _complexAppService.UpdateAsync(id, input);

    /// <summary>Extends a Complex's subscription end date.</summary>
    [HttpPost("{id}/extend-subscription")]
    public Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input) =>
        _complexAppService.ExtendSubscriptionAsync(id, input);

    /// <summary>
    /// Gets the cross-tenant KatKat Score leaderboard (aggregated-only, KVKK privacy shield),
    /// optionally scoped to a district.
    /// </summary>
    [HttpGet("leaderboard")]
    public Task<List<LeaderboardDto>> GetLeaderboardAsync(
        string? district = null, int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount) =>
        _complexAppService.GetLeaderboardAsync(district, maxResultCount);
}
