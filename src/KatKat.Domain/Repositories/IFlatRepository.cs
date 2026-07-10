using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IFlatRepository : IKatKatRepository<Flat, Guid>
{
    Task<bool> FlatNumberExistsInBuildingAsync(Guid buildingId, string flatNumber, Guid? excludedId = null);

    Task<List<Flat>> GetListByBuildingAsync(Guid buildingId);

    /// <summary>
    /// All flats across every Building in the given Complex (joins Building), used to calculate
    /// per-flat Expense shares.
    /// </summary>
    Task<List<Flat>> GetListByComplexAsync(Guid complexId);
}
