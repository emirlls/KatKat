using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Enums;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class SosAlertManager : DomainService
{
    private readonly IComplexRepository _complexRepository;
    private readonly IFlatRepository _flatRepository;

    public SosAlertManager(IComplexRepository complexRepository, IFlatRepository flatRepository)
    {
        _complexRepository = complexRepository;
        _flatRepository = flatRepository;
    }

    public virtual async Task<SosAlert> ReportAsync(Guid complexId, Guid flatId, Guid reporterUserId, SosStatuses status)
    {
        var complex = await _complexRepository.GetAsync(complexId);
        await _flatRepository.GetAsync(flatId);

        return new SosAlert(GuidGenerator.Create(), complex.TenantId, complexId, flatId, reporterUserId, status);
    }
}
