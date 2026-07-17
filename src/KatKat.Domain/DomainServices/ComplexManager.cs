using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class ComplexManager : DomainService
{
    private readonly IComplexRepository _complexRepository;
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public ComplexManager(IComplexRepository complexRepository, INeighborhoodRepository neighborhoodRepository)
    {
        _complexRepository = complexRepository;
        _neighborhoodRepository = neighborhoodRepository;
    }

    /// <summary>
    /// Creates a Complex owned by the tenant currently in scope, or by the host if
    /// multi-tenancy is disabled (see MultiTenancyConsts.IsEnabled).
    /// </summary>
    public virtual async Task<Complex> CreateAsync(
        string name,
        int neighborhoodId,
        string? address,
        decimal latitude,
        decimal longitude,
        DateTime subscriptionStartDate)
    {
        var tenantId = CurrentTenant.Id;

        if (await _complexRepository.NameExistsAsync(name))
        {
            throw new BusinessException(KatKatErrorCodes.ComplexNameAlreadyExists)
                .WithData("name", name);
        }

        await _neighborhoodRepository.GetAsync(neighborhoodId);

        return new Complex(
            GuidGenerator.Create(),
            tenantId,
            name,
            neighborhoodId,
            address,
            latitude,
            longitude,
            subscriptionStartDate
        );
    }
}
