using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.RateLimiting;
using KatKat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Self-registration for new users - no invitation required. IsManager on the request chooses
/// between the Manager role (full KatKat management permissions) and the Resident role (none -
/// residents rely on the self-service endpoints: join a flat, report issues, P2P, reservations, SOS).
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

    [HttpPost("register")]
    [AllowAnonymous]
    public Task RegisterAsync(RegisterDto input) => _accountAppService.RegisterAsync(input);
}
