using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Hubs;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp;
using Volo.Abp.Users;

namespace KatKat.Services;

public class ResourceReservationAppService : KatKatAppService, IResourceReservationAppService
{
    private readonly IResourceReservationRepository _resourceReservationRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ResourceReservationManager _resourceReservationManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public ResourceReservationAppService(
        IResourceReservationRepository resourceReservationRepository,
        IResourceRepository resourceRepository,
        ResourceReservationManager resourceReservationManager,
        IHubContext<KatKatHub> hubContext)
    {
        _resourceReservationRepository = resourceReservationRepository;
        _resourceRepository = resourceRepository;
        _resourceReservationManager = resourceReservationManager;
        _hubContext = hubContext;
    }

    public async Task<ResourceReservationDto> GetAsync(Guid id)
    {
        var reservation = await _resourceReservationRepository.GetAsync(id);
        return ObjectMapper.Map<ResourceReservation, ResourceReservationDto>(reservation);
    }

    public async Task<List<ResourceReservationDto>> GetListByResourceAsync(Guid resourceId)
    {
        var reservations = await _resourceReservationRepository.GetListByResourceAsync(resourceId);
        return reservations.Select(r => ObjectMapper.Map<ResourceReservation, ResourceReservationDto>(r)).ToList();
    }

    public async Task<ResourceReservationDto> CreateAsync(CreateResourceReservationDto input)
    {
        var reservation = await _resourceReservationManager.CreateAsync(
            input.ResourceId, CurrentUser.GetId(), input.StartTime, input.EndTime);

        await _resourceReservationRepository.InsertAsync(reservation, autoSave: true);

        var dto = ObjectMapper.Map<ResourceReservation, ResourceReservationDto>(reservation);

        await BroadcastAsync(reservation.ResourceId, KatKatHubConsts.EventNames.ResourceReservationCreated, dto);

        return dto;
    }

    public async Task<ResourceReservationDto> CancelAsync(Guid id)
    {
        var reservation = await _resourceReservationRepository.GetAsync(id);

        if (reservation.ReservedByUserId != CurrentUser.GetId())
        {
            throw new BusinessException(KatKatErrorCodes.ReservationCanOnlyBeCancelledByReserver);
        }

        reservation.Cancel();
        await _resourceReservationRepository.UpdateAsync(reservation);

        var dto = ObjectMapper.Map<ResourceReservation, ResourceReservationDto>(reservation);

        await BroadcastAsync(reservation.ResourceId, KatKatHubConsts.EventNames.ResourceReservationCancelled, dto);

        return dto;
    }

    private async Task BroadcastAsync(Guid resourceId, string eventName, ResourceReservationDto dto)
    {
        var resource = await _resourceRepository.GetAsync(resourceId);

        await _hubContext.Clients
            .Group(KatKatHub.GroupName(resource.ComplexId))
            .SendAsync(eventName, dto);
    }
}
