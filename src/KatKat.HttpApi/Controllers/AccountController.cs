using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.RateLimiting;
using KatKat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Account provisioning. There is no public self-registration - a Manager account can only be
/// created by the "admin" superuser, and a Resident account can only be created by redeeming a
/// Manager-issued invite (see ResidentInvitationController).
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/account")]
[RateLimit]
public class AccountController : KatKatController, IAccountAppService
{
    private readonly IAccountAppService _accountAppService;

    public AccountController(IAccountAppService accountAppService)
    {
        _accountAppService = accountAppService;
    }

    /// <summary>Admin-only: creates a new Manager account and its own isolated Tenant.</summary>
    [HttpPost("managers")]
    [Authorize(Roles = "admin")]
    public Task CreateManagerAsync(CreateManagerDto input) => _accountAppService.CreateManagerAsync(input);

    /// <summary>Anonymous: resolves a username's Tenant so the login flow can scope the token request.</summary>
    [HttpGet("tenant-lookup")]
    [AllowAnonymous]
    public Task<Guid?> GetTenantIdByUserNameAsync(string userName) =>
        _accountAppService.GetTenantIdByUserNameAsync(userName);

    /// <summary>Admin-only: every Manager across every Tenant, filterable by site location and/or name.</summary>
    [HttpGet("managers")]
    [Authorize(Roles = "admin")]
    public Task<List<ManagerListItemDto>> GetManagersAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount) =>
        _accountAppService.GetManagersAsync(cityId, districtId, neighborhoodId, name, maxResultCount);

    /// <summary>Admin-only: corrects a Manager's username/email/phone.</summary>
    [HttpPut("managers/{tenantId}")]
    [Authorize(Roles = "admin")]
    public Task<ManagerListItemDto> UpdateManagerAsync(Guid tenantId, UpdateManagerDto input) =>
        _accountAppService.UpdateManagerAsync(tenantId, input);
}
