using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

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
        var flat = await GetFlatForNewMemberAsync(flatId, userId);

        return new FlatMember(GuidGenerator.Create(), flat.TenantId, flatId, userId, FlatMemberRole.UnverifiedResident);
    }

    /// <summary>
    /// Registers a user as an already-approved Resident directly - used when the user arrived via
    /// a Manager-issued ResidentInvitation, which already vetted them for this exact Flat, so the
    /// separate UnverifiedResident -&gt; Approve() step InviteAsync uses would be redundant.
    /// </summary>
    public virtual async Task<FlatMember> CreateApprovedAsync(Guid flatId, Guid userId)
    {
        var flat = await GetFlatForNewMemberAsync(flatId, userId);

        return new FlatMember(GuidGenerator.Create(), flat.TenantId, flatId, userId, FlatMemberRole.Resident);
    }

    private async Task<Flat> GetFlatForNewMemberAsync(Guid flatId, Guid userId)
    {
        var flat = await _flatRepository.GetAsync(flatId);

        if (await _flatMemberRepository.ExistsAsync(flatId, userId))
        {
            throw new BusinessException(KatKatErrorCodes.FlatMemberAlreadyExistsForFlat);
        }

        return flat;
    }
}
