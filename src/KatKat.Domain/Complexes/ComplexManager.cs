using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.Complexes;

public class ComplexManager : DomainService
{
    private readonly IComplexRepository _complexRepository;

    public ComplexManager(IComplexRepository complexRepository)
    {
        _complexRepository = complexRepository;
    }

    /// <summary>
    /// Creates the root Complex for the tenant currently in scope. Must be called right
    /// after the tenant itself has been created and switched into (see Onboarding Workflow).
    /// </summary>
    public virtual async Task<Complex> CreateAsync(
        string name,
        string city,
        string district,
        string? address,
        DateTime subscriptionStartDate)
    {
        var tenantId = CurrentTenant.Id
            ?? throw new BusinessException(KatKatErrorCodes.ComplexMustBeCreatedInsideTenantScope);

        if (await _complexRepository.NameExistsAsync(name))
        {
            throw new BusinessException(KatKatErrorCodes.ComplexNameAlreadyExists)
                .WithData("name", name);
        }

        return new Complex(
            GuidGenerator.Create(),
            tenantId,
            name,
            city,
            district,
            address,
            subscriptionStartDate
        );
    }
}
