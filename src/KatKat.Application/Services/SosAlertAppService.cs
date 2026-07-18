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
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace KatKat.Services;

public class SosAlertAppService : KatKatAppService, ISosAlertAppService
{
    private readonly ISosAlertRepository _sosAlertRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly SosAlertManager _sosAlertManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public SosAlertAppService(
        ISosAlertRepository sosAlertRepository, IFlatRepository flatRepository, IBuildingRepository buildingRepository,
        SosAlertManager sosAlertManager, IHubContext<KatKatHub> hubContext)
    {
        _sosAlertRepository = sosAlertRepository;
        _flatRepository = flatRepository;
        _buildingRepository = buildingRepository;
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

        await _sosAlertRepository.InsertAsync(alert, autoSave: true);

        var dto = await MapToDtoAsync(alert);

        await _hubContext.Clients
            .Group(KatKatHub.GroupName(alert.ComplexId))
            .SendAsync(KatKatHubConsts.EventNames.SosAlertReported, dto);

        return dto;
    }

    public async Task<List<SosAlertDto>> GetActiveByComplexAsync(Guid complexId)
    {
        var alerts = await _sosAlertRepository.GetLatestStatusByComplexAsync(complexId);
        return await MapToDtosAsync(alerts);
    }

    public async Task<SosAlertDto> ResolveAsync(Guid id)
    {
        var alert = await _sosAlertRepository.GetAsync(id);

        alert.Resolve(CurrentUser.GetId());

        await _sosAlertRepository.UpdateAsync(alert);

        var dto = await MapToDtoAsync(alert);

        await _hubContext.Clients
            .Group(KatKatHub.GroupName(alert.ComplexId))
            .SendAsync(KatKatHubConsts.EventNames.SosAlertResolved, dto);

        return dto;
    }

    private async Task<SosAlertDto> MapToDtoAsync(SosAlert alert)
    {
        return (await MapToDtosAsync(new List<SosAlert> { alert }))[0];
    }

    /// <summary>Batches the Flat -> FlatNumber and Flat -> Building -> Name lookups for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<SosAlertDto>> MapToDtosAsync(List<SosAlert> alerts)
    {
        if (alerts.Count == 0)
        {
            return new List<SosAlertDto>();
        }

        var flats = await _flatRepository.GetListByIdsAsync(alerts.Select(a => a.FlatId).Distinct());
        var flatById = flats.ToDictionary(f => f.Id);

        var buildings = await _buildingRepository.GetListByIdsAsync(flats.Select(f => f.BuildingId).Distinct());
        var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

        return alerts.Select(alert =>
        {
            var dto = ObjectMapper.Map<SosAlert, SosAlertDto>(alert);
            var flat = flatById.GetValueOrDefault(alert.FlatId);
            dto.FlatNumber = flat?.FlatNumber ?? alert.FlatId.ToString();
            dto.BuildingName = flat != null
                ? buildingNameById.GetValueOrDefault(flat.BuildingId, flat.BuildingId.ToString())
                : alert.FlatId.ToString();
            return dto;
        }).ToList();
    }
}
