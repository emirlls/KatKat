using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Hubs;
using KatKat.P2PRequests;
using KatKat.Repositories;
using Volo.Abp.Users;

namespace KatKat.Services;

public class P2PRequestAppService : KatKatAppService, IP2PRequestAppService
{
    private readonly IP2PRequestRepository _p2pRequestRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly P2PRequestManager _p2pRequestManager;
    private readonly KatKatHubNotifier _hubNotifier;

    public P2PRequestAppService(
        IP2PRequestRepository p2pRequestRepository,
        IFlatRepository flatRepository,
        IBuildingRepository buildingRepository,
        P2PRequestManager p2pRequestManager,
        KatKatHubNotifier hubNotifier)
    {
        _p2pRequestRepository = p2pRequestRepository;
        _flatRepository = flatRepository;
        _buildingRepository = buildingRepository;
        _p2pRequestManager = p2pRequestManager;
        _hubNotifier = hubNotifier;
    }

    public async Task<P2PRequestDto> GetAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);
        return await MapToDtoAsync(request);
    }

    public async Task<List<P2PRequestDto>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null)
    {
        var requests = await _p2pRequestRepository.GetListByComplexAsync(complexId, status);
        return await MapToDtosAsync(requests);
    }

    public async Task<P2PRequestDto> CreateAsync(CreateP2PRequestDto input)
    {
        var requesterUserId = CurrentUser.GetId();
        var requesterFlats = await _flatRepository.GetListByUserAndComplexAsync(requesterUserId, input.ComplexId);
        var flatId = requesterFlats.FirstOrDefault()?.Id;

        var request = await _p2pRequestManager.CreateAsync(
            input.ComplexId, flatId, requesterUserId, input.Title, input.Description, input.NeededUntil);

        await _p2pRequestRepository.InsertAsync(request, autoSave: true);

        var dto = await MapToDtoAsync(request);

        await _hubNotifier.NotifyP2PRequestEventAsync(request.ComplexId, KatKatHubConsts.EventNames.P2PRequestCreated, dto);

        return dto;
    }

    public async Task<P2PRequestDto> FulfillAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);

        request.Fulfill(CurrentUser.GetId());

        // autoSave: true - RecalculateScoreSafelyAsync below re-queries fulfilled-request counts
        // from the database itself, so this update must actually be flushed first or the count
        // (and therefore the Social score) would be computed against the pre-fulfillment state.
        await _p2pRequestRepository.UpdateAsync(request, autoSave: true);

        var dto = await MapToDtoAsync(request);

        await _hubNotifier.NotifyP2PRequestEventAsync(request.ComplexId, KatKatHubConsts.EventNames.P2PRequestFulfilled, dto);
        await RecalculateScoreSafelyAsync(request.ComplexId);

        return dto;
    }

    public async Task<P2PRequestDto> CancelAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);

        request.Cancel();

        await _p2pRequestRepository.UpdateAsync(request);

        var dto = await MapToDtoAsync(request);

        // Tell neighbors the request is gone so their lists drop it without a manual refresh.
        await _hubNotifier.NotifyP2PRequestEventAsync(request.ComplexId, KatKatHubConsts.EventNames.P2PRequestCancelled, dto);

        return dto;
    }

    private async Task<P2PRequestDto> MapToDtoAsync(P2PRequest request)
    {
        return (await MapToDtosAsync(new List<P2PRequest> { request }))[0];
    }

    /// <summary>Batches the Flat -> Building -> Name lookups for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<P2PRequestDto>> MapToDtosAsync(List<P2PRequest> requests)
    {
        if (requests.Count == 0)
        {
            return new List<P2PRequestDto>();
        }

        var flatIds = requests.Where(r => r.FlatId.HasValue).Select(r => r.FlatId!.Value).Distinct();
        var flats = await _flatRepository.GetListByIdsAsync(flatIds);
        var flatById = flats.ToDictionary(f => f.Id);

        var buildingIds = flats.Select(f => f.BuildingId).Distinct();
        var buildings = await _buildingRepository.GetListByIdsAsync(buildingIds);
        var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

        return requests.Select(request =>
        {
            var dto = ObjectMapper.Map<P2PRequest, P2PRequestDto>(request);

            if (request.FlatId.HasValue && flatById.TryGetValue(request.FlatId.Value, out var flat))
            {
                dto.FlatNumber = flat.FlatNumber;
                dto.BuildingId = flat.BuildingId;
                dto.BuildingName = buildingNameById.GetValueOrDefault(flat.BuildingId, flat.BuildingId.ToString());
            }

            return dto;
        }).ToList();
    }
}