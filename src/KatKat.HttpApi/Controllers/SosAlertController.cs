using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Permissions;
using KatKat.RateLimiting;
using KatKat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// The crisis/SOS module: one-tap "Güvendeyim" / "Yardım Lazım" reporting and the manager's
/// real-time digital floor plan.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/sos-alerts")]
[RateLimit]
public class SosAlertController : KatKatController, ISosAlertAppService
{
    private readonly ISosAlertAppService _sosAlertAppService;

    public SosAlertController(ISosAlertAppService sosAlertAppService)
    {
        _sosAlertAppService = sosAlertAppService;
    }

    /// <summary>Reports the current user's status for their flat and broadcasts it immediately.</summary>
    [HttpPost]
    public Task<SosAlertDto> ReportAsync(ReportSosAlertDto input) => _sosAlertAppService.ReportAsync(input);

    /// <summary>Latest status per flat in a Complex - the digital floor plan.</summary>
    [HttpGet]
    public Task<List<SosAlertDto>> GetActiveByComplexAsync(Guid complexId) =>
        _sosAlertAppService.GetActiveByComplexAsync(complexId);

    /// <summary>Manager-only: acknowledges help has reached a HelpNeeded alert.</summary>
    [HttpPost("{id}/resolve")]
    [Authorize(KatKatPermissions.SosAlerts.Resolve)]
    public Task<SosAlertDto> ResolveAsync(Guid id) => _sosAlertAppService.ResolveAsync(id);
}
