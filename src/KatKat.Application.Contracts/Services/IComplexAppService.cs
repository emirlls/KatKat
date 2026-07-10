using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IComplexAppService : IApplicationService
{
    Task<ComplexDto> GetAsync(Guid id);

    Task<ComplexDto> CreateAsync(CreateComplexDto input);

    Task<ComplexDto> UpdateAsync(Guid id, UpdateComplexDto input);

    Task<ComplexDto> ExtendSubscriptionAsync(Guid id, ExtendComplexSubscriptionDto input);

    /// <summary>
    /// Cross-tenant, aggregated-only ranking (KVKK privacy shield) - optionally scoped to a district.
    /// </summary>
    Task<List<LeaderboardDto>> GetLeaderboardAsync(
        string? district = null, int maxResultCount = KatKatConsts.DefaultLeaderboardMaxResultCount);
}
