using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class IssueManager : DomainService
{
    private readonly IComplexRepository _complexRepository;

    public IssueManager(IComplexRepository complexRepository)
    {
        _complexRepository = complexRepository;
    }

    public virtual async Task<Issue> CreateAsync(
        Guid complexId, Guid reporterUserId, string title, string? description, string? photoUrl)
    {
        var complex = await _complexRepository.GetAsync(complexId);

        return new Issue(GuidGenerator.Create(), complex.TenantId, complexId, reporterUserId, title, description, photoUrl);
    }
}
