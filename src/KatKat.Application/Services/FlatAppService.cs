using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;

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

    public async Task<FlatDto> CreateAsync(CreateFlatDto input)
    {
        var flat = await _flatManager.CreateAsync(input.BuildingId, input.FlatNumber, input.FloorNumber, input.ShareFactor);

        await _flatRepository.InsertAsync(flat, autoSave: true);

        return ObjectMapper.Map<Flat, FlatDto>(flat);
    }

    public async Task<FlatDto> UpdateAsync(Guid id, UpdateFlatDto input)
    {
        var flat = await _flatRepository.GetAsync(id);

        flat.SetFlatNumber(input.FlatNumber);
        flat.SetFloorNumber(input.FloorNumber);
        flat.SetShareFactor(input.ShareFactor);

        await _flatRepository.UpdateAsync(flat);

        return ObjectMapper.Map<Flat, FlatDto>(flat);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _flatRepository.DeleteAsync(id);
    }
}
