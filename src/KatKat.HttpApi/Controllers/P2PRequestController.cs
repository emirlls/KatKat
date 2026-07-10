using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.P2PRequests;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// The neighbor-to-neighbor help/item request hub ("Komşu İhtiyaç Hub'ı").
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/p2p-requests")]
public class P2PRequestController : KatKatController, IP2PRequestAppService
{
    private readonly IP2PRequestAppService _p2pRequestAppService;

    public P2PRequestController(IP2PRequestAppService p2pRequestAppService)
    {
        _p2pRequestAppService = p2pRequestAppService;
    }

    /// <summary>Gets a P2P request by id.</summary>
    [HttpGet("{id}")]
    public Task<P2PRequestDto> GetAsync(Guid id) => _p2pRequestAppService.GetAsync(id);

    /// <summary>Lists P2P requests in a Complex, optionally filtered by status.</summary>
    [HttpGet]
    public Task<List<P2PRequestDto>> GetListByComplexAsync(Guid complexId, P2PRequestStatus? status = null) =>
        _p2pRequestAppService.GetListByComplexAsync(complexId, status);

    /// <summary>Creates a new P2P request and pushes it to opted-in neighbors in real time.</summary>
    [HttpPost]
    public Task<P2PRequestDto> CreateAsync(CreateP2PRequestDto input) => _p2pRequestAppService.CreateAsync(input);

    /// <summary>Marks a request as fulfilled by the current user (feeds the Social Score).</summary>
    [HttpPost("{id}/fulfill")]
    public Task<P2PRequestDto> FulfillAsync(Guid id) => _p2pRequestAppService.FulfillAsync(id);

    /// <summary>Cancels an open request.</summary>
    [HttpPost("{id}/cancel")]
    public Task<P2PRequestDto> CancelAsync(Guid id) => _p2pRequestAppService.CancelAsync(id);
}
