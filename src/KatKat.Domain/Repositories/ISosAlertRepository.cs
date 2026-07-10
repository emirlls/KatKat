using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface ISosAlertRepository : IKatKatRepository<SosAlert, Guid>
{
    /// <summary>
    /// The most recent SosAlert per Flat in the Complex - powers the manager's digital floor
    /// plan showing who last reported Safe vs Help Needed.
    /// </summary>
    Task<List<SosAlert>> GetLatestStatusByComplexAsync(Guid complexId);
}
