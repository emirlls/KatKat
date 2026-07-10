using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace KatKat.Buildings;

public interface IBuildingRepository : IRepository<Building, Guid>
{
    Task<bool> NameExistsInComplexAsync(Guid complexId, string name, Guid? excludedId = null);

    Task<List<Building>> GetListByComplexAsync(Guid complexId);
}
