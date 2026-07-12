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
/// Manages the residents/managers attached to a Flat, including the onboarding state machine
/// (UnverifiedResident -&gt; Resident -&gt; Manager).
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/flat-members")]
[RateLimit]
public class FlatMemberController : KatKatController, IFlatMemberAppService
{
    private readonly IFlatMemberAppService _flatMemberAppService;

    public FlatMemberController(IFlatMemberAppService flatMemberAppService)
    {
        _flatMemberAppService = flatMemberAppService;
    }

    /// <summary>Gets a FlatMember by id.</summary>
    [HttpGet("{id}")]
    public Task<FlatMemberDto> GetAsync(Guid id) => _flatMemberAppService.GetAsync(id);

    /// <summary>Lists all members of a Flat.</summary>
    [HttpGet]
    public Task<List<FlatMemberDto>> GetListByFlatAsync(Guid flatId) =>
        _flatMemberAppService.GetListByFlatAsync(flatId);

    /// <summary>Attaches the current user to a Flat as an UnverifiedResident.</summary>
    [HttpPost("invite")]
    public Task<FlatMemberDto> InviteAsync(InviteFlatMemberDto input) => _flatMemberAppService.InviteAsync(input);

    /// <summary>Manager-only: verifies an UnverifiedResident, moving them to Resident.</summary>
    [HttpPost("{id}/approve")]
    [Authorize(KatKatPermissions.FlatMembers.Approve)]
    public Task<FlatMemberDto> ApproveAsync(Guid id) => _flatMemberAppService.ApproveAsync(id);

    /// <summary>Manager-only: promotes an already-verified member to Manager.</summary>
    [HttpPost("{id}/promote-to-manager")]
    [Authorize(KatKatPermissions.FlatMembers.PromoteToManager)]
    public Task<FlatMemberDto> PromoteToManagerAsync(Guid id) => _flatMemberAppService.PromoteToManagerAsync(id);

    /// <summary>Manager-only: removes a member from the flat (e.g. they moved out).</summary>
    [HttpDelete("{id}")]
    [Authorize(KatKatPermissions.FlatMembers.Remove)]
    public Task DeleteAsync(Guid id) => _flatMemberAppService.DeleteAsync(id);

    /// <summary>Manager-only: fetches a member's current username/email/phone, to pre-fill an edit form.</summary>
    [HttpGet("{id}/resident-info")]
    [Authorize(KatKatPermissions.FlatMembers.UpdateResidentInfo)]
    public Task<UpdateResidentInfoDto> GetResidentInfoAsync(Guid id) => _flatMemberAppService.GetResidentInfoAsync(id);

    /// <summary>Manager-only: corrects a member's username/email/phone (e.g. a data-entry mistake).</summary>
    [HttpPut("{id}/resident-info")]
    [Authorize(KatKatPermissions.FlatMembers.UpdateResidentInfo)]
    public Task<FlatMemberDto> UpdateResidentInfoAsync(Guid id, UpdateResidentInfoDto input) =>
        _flatMemberAppService.UpdateResidentInfoAsync(id, input);
}
