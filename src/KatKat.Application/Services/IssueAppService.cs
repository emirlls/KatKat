using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Enums;
using KatKat.Hubs;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace KatKat.Services;

public class IssueAppService : KatKatAppService, IIssueAppService
{
    private readonly IIssueRepository _issueRepository;
    private readonly IssueManager _issueManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public IssueAppService(IIssueRepository issueRepository, IssueManager issueManager, IHubContext<KatKatHub> hubContext)
    {
        _issueRepository = issueRepository;
        _issueManager = issueManager;
        _hubContext = hubContext;
    }

    public async Task<IssueDto> GetAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);
        return ObjectMapper.Map<Issue, IssueDto>(issue);
    }

    public async Task<List<IssueDto>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null)
    {
        var issues = await _issueRepository.GetListByComplexAsync(complexId, status);
        return issues.Select(i => ObjectMapper.Map<Issue, IssueDto>(i)).ToList();
    }

    public async Task<IssueDto> CreateAsync(CreateIssueDto input)
    {
        var issue = await _issueManager.CreateAsync(
            input.ComplexId, CurrentUser.GetId(), input.Title, input.Description, input.PhotoUrl);

        await _issueRepository.InsertAsync(issue);

        return ObjectMapper.Map<Issue, IssueDto>(issue);
    }

    [Authorize(KatKatPermissions.Issues.Resolve)]
    public async Task<IssueDto> StartProgressAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);

        issue.StartProgress();

        await _issueRepository.UpdateAsync(issue);

        return ObjectMapper.Map<Issue, IssueDto>(issue);
    }

    [Authorize(KatKatPermissions.Issues.Resolve)]
    public async Task<IssueDto> ResolveAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);

        issue.Resolve(CurrentUser.GetId());

        await _issueRepository.UpdateAsync(issue);

        var dto = ObjectMapper.Map<Issue, IssueDto>(issue);

        await _hubContext.Clients
            .User(issue.ReporterUserId.ToString())
            .SendAsync(KatKatHubConsts.EventNames.IssueResolved, dto);

        return dto;
    }
}
