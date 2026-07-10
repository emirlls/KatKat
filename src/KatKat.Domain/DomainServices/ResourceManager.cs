using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Repositories;
using KatKat.Resources;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class ResourceManager : DomainService
{
    private readonly IComplexRepository _complexRepository;

    public ResourceManager(IComplexRepository complexRepository)
    {
        _complexRepository = complexRepository;
    }

    public virtual async Task<Resource> CreateAsync(Guid complexId, string name, ResourceType type)
    {
        var complex = await _complexRepository.GetAsync(complexId);

        return new Resource(GuidGenerator.Create(), complex.TenantId, complexId, name, type);
    }
}
