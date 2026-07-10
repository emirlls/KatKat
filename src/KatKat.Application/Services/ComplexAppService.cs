using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace KatKat.Services;

public class ComplexAppService : KatKatAppService, IComplexAppService
{
    private const int LeaderboardCacheMinutes = 15;
    private const string AllDistrictsCacheKey = "all";

    private readonly IComplexRepository _complexRepository;
    private readonly IComplexScoreRepository _complexScoreRepository;
    private readonly ComplexManager _complexManager;
    private readonly IDistributedCache<List<LeaderboardDto>> _leaderboardCache;

    public ComplexAppService(
        IComplexRepository complexRepository,
        IComplexScoreRepository complexScoreRepository,
        ComplexManager complexManager,
        IDistributedCache<List<LeaderboardDto>> leaderboardCache)
    {
        _complexRepository = complexRepository;
        _complexScoreRepository = complexScoreRepository;
        _complexManager = complexManager;
        _leaderboardCache = leaderboardCache;
    }

    public async Task<ComplexDto> GetAsync(Guid id)
    {
        var complex = await _complexRepository.GetAsync(id);
        return ObjectMapper.Map<Complex, ComplexDto>(complex);
    }

    [Authorize(KatKatPermissions.Complexes.Create)]
    public async Task<ComplexDto> CreateAsync(CreateComplexDto input)
    {
        var complex = await _complexManager.CreateAsync(
            input.Name, input.City, input.District, input.Address,
            input.Latitude, input.Longitude, input.SubscriptionStartDate);

        await _complexRepository.InsertAsync(complex);

        return ObjectMapper.Map<Complex, ComplexDto>(complex);
    }

    [Authorize(KatKatPermissions.Complexes.Update)]
    public async Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input)
    {
        var complex = await _complexRepository.GetAsync(id);

        complex.SetName(input.Name);
        complex.SetCity(input.City);
        complex.SetDistrict(input.District);
        complex.SetAddress(input.Address);

        await _complexRepository.UpdateAsync(complex);

        return ObjectMapper.Map<Complex, ComplexDto>(complex);
    }

    [Authorize(KatKatPermissions.Complexes.Update)]
    public async Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input)
    {
        var complex = await _complexRepository.GetAsync(id);

        complex.ExtendSubscription(input.NewEndDate);

        await _complexRepository.UpdateAsync(complex);

        return ObjectMapper.Map<Complex, ComplexDto>(complex);
    }

    public async Task<List<LeaderboardDto>> GetLeaderboardAsync(
        string? district = null, int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount)
    {
        var cacheKey = $"{district ?? AllDistrictsCacheKey}:{maxResultCount}";

        var cached = await _leaderboardCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var scores = await _complexScoreRepository.GetLeaderboardAsync(district, maxResultCount);

        var result = scores.Select((score, index) => new LeaderboardDto
        {
            Rank = index + 1,
            ComplexId = score.ComplexId,
            ComplexName = score.Name,
            City = score.City,
            District = score.District,
            Score = score.TotalScore,
            CalculatedAt = score.CalculatedAt
        }).ToList();

        await _leaderboardCache.SetAsync(cacheKey, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LeaderboardCacheMinutes)
        });

        return result;
    }
}
