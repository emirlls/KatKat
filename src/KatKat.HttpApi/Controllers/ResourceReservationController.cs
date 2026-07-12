using System;
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
/// Hourly bookings of a Resource - live, conflict-free reservations for guest parking and
/// common areas.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/resource-reservations")]
[RateLimit]
public class ResourceReservationController : KatKatController, IResourceReservationAppService
{
    private readonly IResourceReservationAppService _resourceReservationAppService;

    public ResourceReservationController(IResourceReservationAppService resourceReservationAppService)
    {
        _resourceReservationAppService = resourceReservationAppService;
    }

    /// <summary>Gets a reservation by id.</summary>
    [HttpGet("{id}")]
    public Task<ResourceReservationDto> GetAsync(Guid id) => _resourceReservationAppService.GetAsync(id);

    /// <summary>Lists all reservations for a Resource.</summary>
    [HttpGet]
    public Task<List<ResourceReservationDto>> GetListByResourceAsync(Guid resourceId) =>
        _resourceReservationAppService.GetListByResourceAsync(resourceId);

    /// <summary>Requests a booking (starts Pending) - rejected if it overlaps a confirmed reservation.</summary>
    [HttpPost]
    [Authorize(KatKatPermissions.ResourceReservations.Create)]
    public Task<ResourceReservationDto> CreateAsync(CreateResourceReservationDto input) =>
        _resourceReservationAppService.CreateAsync(input);

    /// <summary>Manager-only: approves a pending reservation.</summary>
    [HttpPost("{id}/approve")]
    [Authorize(KatKatPermissions.ResourceReservations.Approve)]
    public Task<ResourceReservationDto> ApproveAsync(Guid id) => _resourceReservationAppService.ApproveAsync(id);

    /// <summary>Manager-only: rejects a pending reservation.</summary>
    [HttpPost("{id}/reject")]
    [Authorize(KatKatPermissions.ResourceReservations.Approve)]
    public Task<ResourceReservationDto> RejectAsync(Guid id) => _resourceReservationAppService.RejectAsync(id);

    /// <summary>Cancels a reservation - only the person who made it may do this.</summary>
    [HttpPost("{id}/cancel")]
    public Task<ResourceReservationDto> CancelAsync(Guid id) => _resourceReservationAppService.CancelAsync(id);
}
