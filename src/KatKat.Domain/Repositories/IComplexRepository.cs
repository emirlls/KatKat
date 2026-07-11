using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IComplexRepository : IKatKatRepository<Complex, Guid>
{
    Task<bool> NameExistsAsync(string name, Guid? excludedId = null);

    Task<bool> ExistsForNeighborhoodAsync(int neighborhoodId);

    /// <summary>
    /// Filters are optional and AND-combined. Only the most specific location filter given is
    /// applied (neighborhoodId, else districtId, else cityId) - there is no point intersecting
    /// them since each narrower filter is already a subset of the wider one.
    /// </summary>
    Task<List<Complex>> SearchAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount);
}
