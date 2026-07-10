using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IComplexScoreRepository : IKatKatRepository<ComplexScore, Guid>
{
    Task<ComplexScore?> FindByComplexIdAsync(Guid complexId);

    /// <summary>
    /// Cross-tenant by design (see ComplexScore remarks) - returns only the aggregated
    /// leaderboard columns callers are allowed to project into a public DTO.
    /// </summary>
    Task<List<ComplexScore>> GetLeaderboardAsync(string? district, int maxResultCount);
}
