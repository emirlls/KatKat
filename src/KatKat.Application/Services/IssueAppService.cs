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
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace KatKat.Services;

public class IssueAppService : KatKatAppService, IIssueAppService
{
    private readonly IIssueRepository _issueRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IssueManager _issueManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public IssueAppService(
        IIssueRepository issueRepository, IBuildingRepository buildingRepository,
        IssueManager issueManager, IHubContext<KatKatHub> hubContext)
    {
        _issueRepository = issueRepository;
        _buildingRepository = buildingRepository;
        _issueManager = issueManager;
        _hubContext = hubContext;
    }

    public async Task<IssueDto> GetAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);
        return await MapToDtoAsync(issue);
    }

    public async Task<List<IssueDto>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null)
    {
        var issues = await _issueRepository.GetListByComplexAsync(complexId, status);
        return await MapToDtosAsync(issues);
    }

    public async Task<IssueDto> CreateAsync(CreateIssueDto input)
    {
        var issue = await _issueManager.CreateAsync(
            input.ComplexId, input.BuildingId, CurrentUser.GetId(), input.Title, input.Description, input.PhotoUrl);

        await _issueRepository.InsertAsync(issue, autoSave: true);

        var dto = await MapToDtoAsync(issue);

        // A newly reported fault shows up live on every complex member's Issues board (managers act on it).
        await BroadcastAsync(issue.ComplexId, KatKatHubConsts.EventNames.IssueCreated, dto);

        return dto;
    }

    public async Task<IssueDto> StartProgressAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);

        issue.StartProgress();

        await _issueRepository.UpdateAsync(issue);

        var dto = await MapToDtoAsync(issue);

        await BroadcastAsync(issue.ComplexId, KatKatHubConsts.EventNames.IssueInProgress, dto);

        return dto;
    }

    public async Task<IssueDto> ResolveAsync(Guid id)
    {
        var issue = await _issueRepository.GetAsync(id);

        issue.Resolve(CurrentUser.GetId());

        // autoSave: true - RecalculateScoreSafelyAsync below re-queries resolution delays from the
        // database itself, so this update must actually be flushed first or the just-resolved
        // issue wouldn't be counted yet.
        await _issueRepository.UpdateAsync(issue, autoSave: true);

        var dto = await MapToDtoAsync(issue);

        await BroadcastAsync(issue.ComplexId, KatKatHubConsts.EventNames.IssueResolved, dto);
        await RecalculateScoreSafelyAsync(issue.ComplexId);

        return dto;
    }

    private Task BroadcastAsync(Guid complexId, string eventName, IssueDto dto)
    {
        return _hubContext.Clients.Group(KatKatHub.GroupName(complexId)).SendAsync(eventName, dto);
    }

    private async Task<IssueDto> MapToDtoAsync(Issue issue)
    {
        return (await MapToDtosAsync(new List<Issue> { issue }))[0];
    }

    /// <summary>Batches the Building -> Name lookup for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<IssueDto>> MapToDtosAsync(List<Issue> issues)
    {
        if (issues.Count == 0)
        {
            return new List<IssueDto>();
        }

        var buildingIds = issues.Where(i => i.BuildingId.HasValue).Select(i => i.BuildingId!.Value).Distinct();
        var buildings = await _buildingRepository.GetListByIdsAsync(buildingIds);
        var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

        return issues.Select(issue =>
        {
            var dto = ObjectMapper.Map<Issue, IssueDto>(issue);
            dto.BuildingName = issue.BuildingId.HasValue
                ? buildingNameById.GetValueOrDefault(issue.BuildingId.Value, issue.BuildingId.Value.ToString())
                : null;
            return dto;
        }).ToList();
    }
}
