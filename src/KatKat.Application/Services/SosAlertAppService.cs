using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Hubs;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace KatKat.Services;

public class SosAlertAppService : KatKatAppService, ISosAlertAppService
{
    private readonly ISosAlertRepository _sosAlertRepository;
    private readonly SosAlertManager _sosAlertManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public SosAlertAppService(ISosAlertRepository sosAlertRepository, SosAlertManager sosAlertManager, IHubContext<KatKatHub> hubContext)
    {
        _sosAlertRepository = sosAlertRepository;
        _sosAlertManager = sosAlertManager;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Unlike P2P request pushes, SOS alerts are never opt-out-able - they go to the whole
    /// Complex group unconditionally, life-safety information can't be muted.
    /// </summary>
    public async Task<SosAlertDto> ReportAsync(ReportSosAlertDto input)
    {
        var alert = await _sosAlertManager.ReportAsync(input.ComplexId, input.FlatId, CurrentUser.GetId(), input.Status);

        await _sosAlertRepository.InsertAsync(alert);

        var dto = ObjectMapper.Map<SosAlert, SosAlertDto>(alert);

        await _hubContext.Clients
            .Group(KatKatHub.GroupName(alert.ComplexId))
            .SendAsync(KatKatHubConsts.EventNames.SosAlertReported, dto);

        return dto;
    }

    public async Task<List<SosAlertDto>> GetActiveByComplexAsync(Guid complexId)
    {
        var alerts = await _sosAlertRepository.GetLatestStatusByComplexAsync(complexId);
        return alerts.Select(a => ObjectMapper.Map<SosAlert, SosAlertDto>(a)).ToList();
    }

    [Authorize(KatKatPermissions.SosAlerts.Resolve)]
    public async Task<SosAlertDto> ResolveAsync(Guid id)
    {
        var alert = await _sosAlertRepository.GetAsync(id);

        alert.Resolve(CurrentUser.GetId());

        await _sosAlertRepository.UpdateAsync(alert);

        var dto = ObjectMapper.Map<SosAlert, SosAlertDto>(alert);

        await _hubContext.Clients
            .Group(KatKatHub.GroupName(alert.ComplexId))
            .SendAsync(KatKatHubConsts.EventNames.SosAlertResolved, dto);

        return dto;
    }
}
