using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace KatKat.Services;

public class FlatAppService : KatKatAppService, IFlatAppService
{
    private readonly IFlatRepository _flatRepository;
    private readonly FlatManager _flatManager;

    public FlatAppService(IFlatRepository flatRepository, FlatManager flatManager)
    {
        _flatRepository = flatRepository;
        _flatManager = flatManager;
    }

    public async Task<FlatDto> GetAsync(Guid id)
    {
        var flat = await _flatRepository.GetAsync(id);
        return ObjectMapper.Map<Flat, FlatDto>(flat);
    }

    public async Task<List<FlatDto>> GetListByBuildingAsync(Guid buildingId)
    {
        var flats = await _flatRepository.GetListByBuildingAsync(buildingId);
        return flats.Select(f => ObjectMapper.Map<Flat, FlatDto>(f)).ToList();
    }

    [Authorize(KatKatPermissions.Flats.Create)]
    public async Task<FlatDto> CreateAsync(CreateFlatDto input)
    {
        var flat = await _flatManager.CreateAsync(input.BuildingId, input.FlatNumber, input.FloorNumber, input.ShareFactor);

        await _flatRepository.InsertAsync(flat);

        return ObjectMapper.Map<Flat, FlatDto>(flat);
    }
}
