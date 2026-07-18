using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Repositories;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace KatKat.Services;

public class ResidentInvitationAppService : KatKatAppService, IResidentInvitationAppService
{
    private readonly IResidentInvitationRepository _residentInvitationRepository;
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly ResidentInvitationManager _residentInvitationManager;
    private readonly FlatMemberManager _flatMemberManager;
    private readonly IdentityUserManager _userManager;
    private readonly IGuidGenerator _guidGenerator;

    public ResidentInvitationAppService(
        IResidentInvitationRepository residentInvitationRepository,
        IFlatMemberRepository flatMemberRepository,
        ResidentInvitationManager residentInvitationManager,
        FlatMemberManager flatMemberManager,
        IdentityUserManager userManager,
        IGuidGenerator guidGenerator)
    {
        _residentInvitationRepository = residentInvitationRepository;
        _flatMemberRepository = flatMemberRepository;
        _residentInvitationManager = residentInvitationManager;
        _flatMemberManager = flatMemberManager;
        _userManager = userManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<ResidentInvitationDto> CreateAsync(CreateResidentInvitationDto input)
    {
        var invitation = await _residentInvitationManager.CreateAsync(input.FlatId, CurrentUser.GetId());

        await _residentInvitationRepository.InsertAsync(invitation, autoSave: true);

        return ObjectMapper.Map<ResidentInvitation, ResidentInvitationDto>(invitation);
    }

    public async Task RedeemAsync(RedeemResidentInvitationDto input)
    {
        var invitation = await _residentInvitationRepository.FindByCodeAsync(input.Code)
            ?? throw new EntityNotFoundException(typeof(ResidentInvitation));

        // Everything below must land in the invitation's own Tenant - the caller is anonymous and
        // has no tenant context of their own yet, this invitation is what grants them one.
        using (CurrentTenant.Change(invitation.TenantId))
        {
            var user = new Volo.Abp.Identity.IdentityUser(_guidGenerator.Create(), input.UserName, input.Email, invitation.TenantId)
            {
                Name = input.Name,
                Surname = input.Surname,
            };
            user.SetPhoneNumber(input.PhoneNumber, confirmed: false);
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();
            (await _userManager.AddToRoleAsync(user, KatKatRoleConsts.ResidentRoleName)).CheckErrors();

            var flatMember = await _flatMemberManager.CreateApprovedAsync(invitation.FlatId, user.Id);
            await _flatMemberRepository.InsertAsync(flatMember, autoSave: true);

            invitation.MarkRedeemed(user.Id, Clock.Now);
            await _residentInvitationRepository.UpdateAsync(invitation, autoSave: true);
        }
    }
}
