using System;
using System.Threading.Tasks;
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
}
