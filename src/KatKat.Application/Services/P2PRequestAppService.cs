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
    private readonly P2PRequestManager _p2pRequestManager;
    private readonly KatKatHubNotifier _hubNotifier;

    public P2PRequestAppService(
        IP2PRequestRepository p2pRequestRepository,
        P2PRequestManager p2pRequestManager,
        KatKatHubNotifier hubNotifier)
    {
        _p2pRequestRepository = p2pRequestRepository;
        _p2pRequestManager = p2pRequestManager;
        _hubNotifier = hubNotifier;
    }

    public async Task<P2PRequestDto> GetAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);
        return ObjectMapper.Map<P2PRequest, P2PRequestDto>(request);
    }

    public async Task<List<P2PRequestDto>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null)
    {
        var requests = await _p2pRequestRepository.GetListByComplexAsync(complexId, status);
        return requests.Select(r => ObjectMapper.Map<P2PRequest, P2PRequestDto>(r)).ToList();
    }

    public async Task<P2PRequestDto> CreateAsync(CreateP2PRequestDto input)
    {
        var request = await _p2pRequestManager.CreateAsync(
            input.ComplexId, CurrentUser.GetId(), input.Title, input.Description, input.NeededUntil);

        await _p2pRequestRepository.InsertAsync(request);

        var dto = ObjectMapper.Map<P2PRequest, P2PRequestDto>(request);

        await _hubNotifier.NotifyP2PRequestEventAsync(request.ComplexId, KatKatHubConsts.EventNames.P2PRequestCreated, dto);

        return dto;
    }

    public async Task<P2PRequestDto> FulfillAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);

        request.Fulfill(CurrentUser.GetId());

        await _p2pRequestRepository.UpdateAsync(request);

        var dto = ObjectMapper.Map<P2PRequest, P2PRequestDto>(request);

        await _hubNotifier.NotifyP2PRequestEventAsync(request.ComplexId, KatKatHubConsts.EventNames.P2PRequestFulfilled, dto);

        return dto;
    }

    public async Task<P2PRequestDto> CancelAsync(Guid id)
    {
        var request = await _p2pRequestRepository.GetAsync(id);

        request.Cancel();

        await _p2pRequestRepository.UpdateAsync(request);

        return ObjectMapper.Map<P2PRequest, P2PRequestDto>(request);
    }
}