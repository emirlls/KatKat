using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class IssueManager : DomainService
{
    private readonly IComplexRepository _complexRepository;
    private readonly IBuildingRepository _buildingRepository;

    public IssueManager(IComplexRepository complexRepository, IBuildingRepository buildingRepository)
    {
        _complexRepository = complexRepository;
        _buildingRepository = buildingRepository;
    }

    public virtual async Task<Issue> CreateAsync(
        Guid complexId, Guid? buildingId, Guid reporterUserId, string title, string? description, string? photoUrl)
    {
        var complex = await _complexRepository.GetAsync(complexId);

        if (buildingId.HasValue)
        {
            var building = await _buildingRepository.GetAsync(buildingId.Value);
            if (building.ComplexId != complexId)
            {
                throw new BusinessException(KatKatErrorCodes.IssueBuildingMustBelongToComplex);
            }
        }

        return new Issue(GuidGenerator.Create(), complex.TenantId, complexId, buildingId, reporterUserId, title, description, photoUrl);
    }
}
