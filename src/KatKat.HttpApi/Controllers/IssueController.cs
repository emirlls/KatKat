using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Enums;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Digital fault/complaint reporting ("Dijital Arıza &amp; Şikayet Yönetimi").
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/issues")]
public class IssueController : KatKatController, IIssueAppService
{
    private readonly IIssueAppService _issueAppService;

    public IssueController(IIssueAppService issueAppService)
    {
        _issueAppService = issueAppService;
    }

    /// <summary>Gets an Issue by id.</summary>
    [HttpGet("{id}")]
    public Task<IssueDto> GetAsync(Guid id) => _issueAppService.GetAsync(id);

    /// <summary>Lists Issues in a Complex, optionally filtered by status.</summary>
    [HttpGet]
    public Task<List<IssueDto>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null) =>
        _issueAppService.GetListByComplexAsync(complexId, status);

    /// <summary>Reports a new issue as the current user.</summary>
    [HttpPost]
    public Task<IssueDto> CreateAsync(CreateIssueDto input) => _issueAppService.CreateAsync(input);

    /// <summary>Manager-only: marks an open issue as being worked on.</summary>
    [HttpPost("{id}/start-progress")]
    public Task<IssueDto> StartProgressAsync(Guid id) => _issueAppService.StartProgressAsync(id);

    /// <summary>Manager-only: marks an in-progress issue as resolved and notifies the reporter.</summary>
    [HttpPost("{id}/resolve")]
    public Task<IssueDto> ResolveAsync(Guid id) => _issueAppService.ResolveAsync(id);
}
