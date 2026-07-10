using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Enums;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IIssueAppService : IApplicationService
{
    Task<IssueDto> GetAsync(Guid id);

    Task<List<IssueDto>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null);

    /// <summary>Reports a new issue as the current user.</summary>
    Task<IssueDto> CreateAsync(CreateIssueDto input);

    /// <summary>Manager-only: marks an open issue as being worked on.</summary>
    Task<IssueDto> StartProgressAsync(Guid id);

    /// <summary>Manager-only: marks an in-progress issue as resolved (feeds the Resolution Score).</summary>
    Task<IssueDto> ResolveAsync(Guid id);
}
