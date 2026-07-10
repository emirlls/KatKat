using System;
using System.Threading.Tasks;
using KatKat.Flats;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.FlatMembers;

public class FlatMemberManager : DomainService
{
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly IFlatRepository _flatRepository;

    public FlatMemberManager(IFlatMemberRepository flatMemberRepository, IFlatRepository flatRepository)
    {
        _flatMemberRepository = flatMemberRepository;
        _flatRepository = flatRepository;
    }

    /// <summary>
    /// Registers a user against a flat as an UnverifiedResident (read-only state machine entry
    /// point of the WhatsApp Invitation onboarding workflow).
    /// </summary>
    public virtual async Task<FlatMember> InviteAsync(Guid flatId, Guid userId)
    {
        var flat = await _flatRepository.GetAsync(flatId);

        if (await _flatMemberRepository.ExistsAsync(flatId, userId))
        {
            throw new BusinessException(KatKatErrorCodes.FlatMemberAlreadyExistsForFlat);
        }

        return new FlatMember(GuidGenerator.Create(), flat.TenantId, flatId, userId, FlatMemberRole.UnverifiedResident);
    }
}
