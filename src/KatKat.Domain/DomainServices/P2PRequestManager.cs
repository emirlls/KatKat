using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class P2PRequestManager : DomainService
{
    private readonly IComplexRepository _complexRepository;

    public P2PRequestManager(IComplexRepository complexRepository)
    {
        _complexRepository = complexRepository;
    }

    /// <summary>
    /// Creating a request touches another aggregate (Complex, to resolve the tenant and confirm
    /// it exists), so it belongs here rather than directly in the Application Service. Fulfilling
    /// or cancelling a request is a single-aggregate state change and is done via the entity's own
    /// methods from the Application Service.
    /// </summary>
    public virtual async Task<P2PRequest> CreateAsync(
        Guid complexId,
        Guid? flatId,
        Guid requesterUserId,
        string title,
        string? description,
        DateTime? neededUntil)
    {
        var complex = await _complexRepository.GetAsync(complexId);

        return new P2PRequest(GuidGenerator.Create(), complex.TenantId, complexId, flatId, requesterUserId, title, description, neededUntil);
    }
}
