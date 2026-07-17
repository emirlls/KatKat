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

public class ResourceAppService : KatKatAppService, IResourceAppService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ResourceManager _resourceManager;

    public ResourceAppService(IResourceRepository resourceRepository, ResourceManager resourceManager)
    {
        _resourceRepository = resourceRepository;
        _resourceManager = resourceManager;
    }

    public async Task<ResourceDto> GetAsync(Guid id)
    {
        var resource = await _resourceRepository.GetAsync(id);
        return ObjectMapper.Map<Resource, ResourceDto>(resource);
    }

    public async Task<List<ResourceDto>> GetListByComplexAsync(Guid complexId)
    {
        var resources = await _resourceRepository.GetListByComplexAsync(complexId);
        return resources.Select(r => ObjectMapper.Map<Resource, ResourceDto>(r)).ToList();
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceDto input)
    {
        var resource = await _resourceManager.CreateAsync(input.ComplexId, input.Name, input.Type);

        await _resourceRepository.InsertAsync(resource, autoSave: true);

        return ObjectMapper.Map<Resource, ResourceDto>(resource);
    }
}
