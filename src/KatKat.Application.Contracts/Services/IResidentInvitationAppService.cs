using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IResidentInvitationAppService : IApplicationService
{
    /// <summary>Manager-only: generates a redeemable invite code for a specific Flat.</summary>
    Task<ResidentInvitationDto> CreateAsync(CreateResidentInvitationDto input);

    /// <summary>
    /// Public: redeems a code by creating the resident's own account (scoped to the invitation's
    /// Tenant/site) and attaching them to the invited Flat as an already-approved Resident.
    /// </summary>
    Task RedeemAsync(RedeemResidentInvitationDto input);
}
