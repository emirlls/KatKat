using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Enums;

namespace KatKat.Repositories;

public interface IIssueRepository : IKatKatRepository<Issue, Guid>
{
    Task<List<Issue>> GetListByComplexAsync(Guid complexId, IssueStatuses? status = null);

    /// <summary>
    /// Average hours between an Issue's report time and its ResolvedAt, across issues resolved
    /// since <paramref name="since"/>. Null if nothing has been resolved yet.
    /// </summary>
    Task<decimal?> GetAverageResolutionHoursAsync(Guid complexId, DateTime since);
}
