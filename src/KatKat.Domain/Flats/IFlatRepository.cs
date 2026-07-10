using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace KatKat.Flats;

public interface IFlatRepository : IRepository<Flat, Guid>
{
    Task<bool> FlatNumberExistsInBuildingAsync(Guid buildingId, string flatNumber, Guid? excludedId = null);

    Task<List<Flat>> GetListByBuildingAsync(Guid buildingId);
}
