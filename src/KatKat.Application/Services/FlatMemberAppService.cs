using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace KatKat.Services;

public class FlatMemberAppService : KatKatAppService, IFlatMemberAppService
{
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly FlatMemberManager _flatMemberManager;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IdentityUserManager _identityUserManager;

    public FlatMemberAppService(
        IFlatMemberRepository flatMemberRepository, FlatMemberManager flatMemberManager,
        IIdentityUserRepository identityUserRepository, IdentityUserManager identityUserManager)
    {
        _flatMemberRepository = flatMemberRepository;
        _flatMemberManager = flatMemberManager;
        _identityUserRepository = identityUserRepository;
        _identityUserManager = identityUserManager;
    }

    public async Task<FlatMemberDto> GetAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);
        return await MapToDtoAsync(flatMember);
    }

    public async Task<List<FlatMemberDto>> GetListByFlatAsync(Guid flatId)
    {
        var flatMembers = await _flatMemberRepository.GetListByFlatAsync(flatId);
        return await MapToDtosAsync(flatMembers);
    }

    public async Task<FlatMemberDto> InviteAsync(InviteFlatMemberDto input)
    {
        var flatMember = await _flatMemberManager.InviteAsync(input.FlatId, CurrentUser.GetId());

        await _flatMemberRepository.InsertAsync(flatMember, autoSave: true);

        return await MapToDtoAsync(flatMember);
    }

    public async Task<FlatMemberDto> ApproveAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);

        flatMember.Approve();

        await _flatMemberRepository.UpdateAsync(flatMember, autoSave: true);

        return await MapToDtoAsync(flatMember);
    }

    public async Task<FlatMemberDto> PromoteToManagerAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);

        flatMember.PromoteToManager();

        await _flatMemberRepository.UpdateAsync(flatMember, autoSave: true);

        return await MapToDtoAsync(flatMember);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _flatMemberRepository.DeleteAsync(id, autoSave: true);
    }

    public async Task<UpdateResidentInfoDto> GetResidentInfoAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);
        var user = await _identityUserRepository.GetAsync(flatMember.UserId);

        return new UpdateResidentInfoDto
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
        };
    }

    public async Task<FlatMemberDto> UpdateResidentInfoAsync(Guid id, UpdateResidentInfoDto input)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);
        var user = await _identityUserRepository.GetAsync(flatMember.UserId);

        (await _identityUserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
        (await _identityUserManager.SetEmailAsync(user, input.Email)).CheckErrors();
        (await _identityUserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();

        return await MapToDtoAsync(flatMember);
    }

    private async Task<FlatMemberDto> MapToDtoAsync(FlatMember flatMember)
    {
        return (await MapToDtosAsync(new List<FlatMember> { flatMember }))[0];
    }

    /// <summary>Batches the User -> UserName lookup for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<FlatMemberDto>> MapToDtosAsync(List<FlatMember> flatMembers)
    {
        if (flatMembers.Count == 0)
        {
            return new List<FlatMemberDto>();
        }

        var users = await _identityUserRepository.GetListByIdsAsync(flatMembers.Select(fm => fm.UserId).Distinct());
        var userNameById = users.ToDictionary(u => u.Id, u => u.UserName);

        return flatMembers.Select(flatMember =>
        {
            var dto = ObjectMapper.Map<FlatMember, FlatMemberDto>(flatMember);
            dto.UserName = userNameById.GetValueOrDefault(flatMember.UserId, flatMember.UserId.ToString());
            return dto;
        }).ToList();
    }
}
