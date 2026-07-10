using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IBuildingRepository : IKatKatRepository<Building, Guid>
{
    Task<bool> NameExistsInComplexAsync(Guid complexId, string name, Guid? excludedId = null);

    Task<List<Building>> GetListByComplexAsync(Guid complexId);
}
