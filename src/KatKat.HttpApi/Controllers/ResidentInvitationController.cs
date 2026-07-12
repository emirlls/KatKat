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
/// Manager-issued, one-time codes that let a resident create their own account for a specific
/// Flat without ever self-registering.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/resident-invitations")]
[RateLimit]
public class ResidentInvitationController : KatKatController, IResidentInvitationAppService
{
    private readonly IResidentInvitationAppService _residentInvitationAppService;

    public ResidentInvitationController(IResidentInvitationAppService residentInvitationAppService)
    {
        _residentInvitationAppService = residentInvitationAppService;
    }

    /// <summary>Manager-only: generates a redeemable invite code for a specific Flat.</summary>
    [HttpPost]
    [Authorize(KatKatPermissions.FlatMembers.Invite)]
    public Task<ResidentInvitationDto> CreateAsync(CreateResidentInvitationDto input) =>
        _residentInvitationAppService.CreateAsync(input);

    /// <summary>Public: redeems a code by creating the resident's own account.</summary>
    [HttpPost("redeem")]
    [AllowAnonymous]
    public Task RedeemAsync(RedeemResidentInvitationDto input) => _residentInvitationAppService.RedeemAsync(input);
}
