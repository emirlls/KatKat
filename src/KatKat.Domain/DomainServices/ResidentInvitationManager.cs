using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;

namespace KatKat.DomainServices;

public class ResidentInvitationManager : DomainService
{
    private readonly IFlatRepository _flatRepository;

    public ResidentInvitationManager(IFlatRepository flatRepository)
    {
        _flatRepository = flatRepository;
    }

    public virtual async Task<ResidentInvitation> CreateAsync(Guid flatId, Guid createdByUserId)
    {
        var flat = await _flatRepository.GetAsync(flatId);

        var code = Guid.NewGuid().ToString("N");
        var expiresAt = Clock.Now.AddDays(ResidentInvitationConsts.ValidityDays);

        return new ResidentInvitation(GuidGenerator.Create(), flat.TenantId, flatId, code, createdByUserId, expiresAt);
    }
}
