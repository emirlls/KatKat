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
    private readonly IFlatRepository _flatRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly ResourceReservationManager _resourceReservationManager;
    private readonly IHubContext<KatKatHub> _hubContext;

    public ResourceReservationAppService(
        IResourceReservationRepository resourceReservationRepository,
        IResourceRepository resourceRepository,
        IFlatRepository flatRepository,
        IBuildingRepository buildingRepository,
        ResourceReservationManager resourceReservationManager,
        IHubContext<KatKatHub> hubContext)
    {
        _resourceReservationRepository = resourceReservationRepository;
        _resourceRepository = resourceRepository;
        _flatRepository = flatRepository;
        _buildingRepository = buildingRepository;
        _resourceReservationManager = resourceReservationManager;
        _hubContext = hubContext;
    }

    public async Task<ResourceReservationDto> GetAsync(Guid id)
    {
        var reservation = await _resourceReservationRepository.GetAsync(id);
        return await MapToDtoAsync(reservation);
    }

    public async Task<List<ResourceReservationDto>> GetListByResourceAsync(Guid resourceId)
    {
        var reservations = await _resourceReservationRepository.GetListByResourceAsync(resourceId);
        return await MapToDtosAsync(reservations);
    }

    public async Task<ResourceReservationDto> CreateAsync(CreateResourceReservationDto input)
    {
        var reservedByUserId = CurrentUser.GetId();
        var resource = await _resourceRepository.GetAsync(input.ResourceId);
        var reserverFlats = await _flatRepository.GetListByUserAndComplexAsync(reservedByUserId, resource.ComplexId);
        var flatId = reserverFlats.FirstOrDefault()?.Id;

        var reservation = await _resourceReservationManager.CreateAsync(
            input.ResourceId, flatId, reservedByUserId, input.StartTime, input.EndTime);

        await _resourceReservationRepository.InsertAsync(reservation, autoSave: true);

        var dto = await MapToDtoAsync(reservation);

        // Broadcast the pending request to the whole complex so managers see it and can act.
        await BroadcastAsync(reservation.ResourceId, KatKatHubConsts.EventNames.ResourceReservationCreated, dto);

        return dto;
    }

    public async Task<ResourceReservationDto> ApproveAsync(Guid id)
    {
        var reservation = await _resourceReservationRepository.GetAsync(id);

        await _resourceReservationManager.ApproveAsync(reservation);
        await _resourceReservationRepository.UpdateAsync(reservation);

        var dto = await MapToDtoAsync(reservation);

        // Complex-wide refresh (calendars) + a direct nudge to the person who made the request.
        await BroadcastAsync(reservation.ResourceId, KatKatHubConsts.EventNames.ResourceReservationApproved, dto);
        await NotifyReserverAsync(reservation.ReservedByUserId, KatKatHubConsts.EventNames.ResourceReservationApproved, dto);

        return dto;
    }

    public async Task<ResourceReservationDto> RejectAsync(Guid id, RejectResourceReservationDto input)
    {
        var reservation = await _resourceReservationRepository.GetAsync(id);

        reservation.Reject(input.Reason);
        await _resourceReservationRepository.UpdateAsync(reservation);

        var dto = await MapToDtoAsync(reservation);

        await BroadcastAsync(reservation.ResourceId, KatKatHubConsts.EventNames.ResourceReservationRejected, dto);
        await NotifyReserverAsync(reservation.ReservedByUserId, KatKatHubConsts.EventNames.ResourceReservationRejected, dto);

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

        var dto = await MapToDtoAsync(reservation);

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

    private Task NotifyReserverAsync(Guid reservedByUserId, string eventName, ResourceReservationDto dto)
    {
        return _hubContext.Clients.User(reservedByUserId.ToString()).SendAsync(eventName, dto);
    }

    private async Task<ResourceReservationDto> MapToDtoAsync(ResourceReservation reservation)
    {
        return (await MapToDtosAsync(new List<ResourceReservation> { reservation }))[0];
    }

    /// <summary>Batches the Flat -> Building -> Name lookups for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<ResourceReservationDto>> MapToDtosAsync(List<ResourceReservation> reservations)
    {
        if (reservations.Count == 0)
        {
            return new List<ResourceReservationDto>();
        }

        var flatIds = reservations.Where(r => r.FlatId.HasValue).Select(r => r.FlatId!.Value).Distinct();
        var flats = await _flatRepository.GetListByIdsAsync(flatIds);
        var flatById = flats.ToDictionary(f => f.Id);

        var buildingIds = flats.Select(f => f.BuildingId).Distinct();
        var buildings = await _buildingRepository.GetListByIdsAsync(buildingIds);
        var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

        return reservations.Select(reservation =>
        {
            var dto = ObjectMapper.Map<ResourceReservation, ResourceReservationDto>(reservation);

            if (reservation.FlatId.HasValue && flatById.TryGetValue(reservation.FlatId.Value, out var flat))
            {
                dto.FlatNumber = flat.FlatNumber;
                dto.BuildingId = flat.BuildingId;
                dto.BuildingName = buildingNameById.GetValueOrDefault(flat.BuildingId, flat.BuildingId.ToString());
            }

            return dto;
        }).ToList();
    }
}
