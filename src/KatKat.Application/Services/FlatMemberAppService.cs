using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Users;

namespace KatKat.Services;

public class FlatMemberAppService : KatKatAppService, IFlatMemberAppService
{
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly FlatMemberManager _flatMemberManager;

    public FlatMemberAppService(IFlatMemberRepository flatMemberRepository, FlatMemberManager flatMemberManager)
    {
        _flatMemberRepository = flatMemberRepository;
        _flatMemberManager = flatMemberManager;
    }

    public async Task<FlatMemberDto> GetAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);
        return ObjectMapper.Map<FlatMember, FlatMemberDto>(flatMember);
    }

    public async Task<List<FlatMemberDto>> GetListByFlatAsync(Guid flatId)
    {
        var flatMembers = await _flatMemberRepository.GetListByFlatAsync(flatId);
        return flatMembers.Select(fm => ObjectMapper.Map<FlatMember, FlatMemberDto>(fm)).ToList();
    }

    public async Task<FlatMemberDto> InviteAsync(InviteFlatMemberDto input)
    {
        var flatMember = await _flatMemberManager.InviteAsync(input.FlatId, CurrentUser.GetId());

        await _flatMemberRepository.InsertAsync(flatMember, autoSave: true);

        return ObjectMapper.Map<FlatMember, FlatMemberDto>(flatMember);
    }

    public async Task<FlatMemberDto> ApproveAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);

        flatMember.Approve();

        await _flatMemberRepository.UpdateAsync(flatMember);

        return ObjectMapper.Map<FlatMember, FlatMemberDto>(flatMember);
    }

    public async Task<FlatMemberDto> PromoteToManagerAsync(Guid id)
    {
        var flatMember = await _flatMemberRepository.GetAsync(id);

        flatMember.PromoteToManager();

        await _flatMemberRepository.UpdateAsync(flatMember);

        return ObjectMapper.Map<FlatMember, FlatMemberDto>(flatMember);
    }
}
