using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Permissions;
using KatKat.RateLimiting;
using KatKat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Manages Complexes (apartment sites) - the root tenant entity of KatKat.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/complexes")]
[RateLimit]
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
    [Authorize(KatKatPermissions.Complexes.Create)]
    public Task<ComplexDto> CreateAsync(CreateComplexDto input) => _complexAppService.CreateAsync(input);

    /// <summary>Updates a Complex's basic info (name, neighborhood, address).</summary>
    [HttpPut("{id}")]
    [Authorize(KatKatPermissions.Complexes.Update)]
    public Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input) => _complexAppService.UpdateAsync(id, input);

    /// <summary>Extends a Complex's subscription end date.</summary>
    [HttpPost("{id}/extend-subscription")]
    [Authorize(KatKatPermissions.Complexes.Update)]
    public Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input) =>
        _complexAppService.ExtendSubscriptionAsync(id, input);

    /// <summary>
    /// Gets the cross-tenant KatKat Score leaderboard (aggregated-only, KVKK privacy shield),
    /// optionally scoped to a district and/or neighborhood.
    /// </summary>
    [HttpGet("leaderboard")]
    public Task<List<LeaderboardDto>> GetLeaderboardAsync(
        int? districtId = null, int? neighborhoodId = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount) =>
        _complexAppService.GetLeaderboardAsync(districtId, neighborhoodId, maxResultCount);

    /// <summary>Gets nearby buildings' KatKat Scores within a radius (km) of a coordinate, for map rendering.</summary>
    [HttpGet("nearby-leaderboard")]
    public Task<List<LeaderboardDto>> GetNearbyLeaderboardAsync(
        decimal latitude, decimal longitude, double? radiusKm = null,
        int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount) =>
        _complexAppService.GetNearbyLeaderboardAsync(latitude, longitude, radiusKm, maxResultCount);

    /// <summary>Gets every district's own leaderboard, grouped together in one response.</summary>
    [HttpGet("leaderboard/by-district")]
    public Task<List<DistrictLeaderboardDto>> GetAllDistrictLeaderboardsAsync(
        int maxResultCountPerDistrict = KatKatConsts.DefaultLeaderboardMaxResultCount) =>
        _complexAppService.GetAllDistrictLeaderboardsAsync(maxResultCountPerDistrict);

    /// <summary>Gets every neighborhood's own leaderboard, grouped together in one response.</summary>
    [HttpGet("leaderboard/by-neighborhood")]
    public Task<List<NeighborhoodLeaderboardDto>> GetAllNeighborhoodLeaderboardsAsync(
        int maxResultCountPerNeighborhood = KatKatConsts.DefaultLeaderboardMaxResultCount) =>
        _complexAppService.GetAllNeighborhoodLeaderboardsAsync(maxResultCountPerNeighborhood);
}
