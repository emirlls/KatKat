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
    /// them since each narrower filter is already a subset of the wider one. Tenant-scoped as
    /// usual (only the caller's own Complex(es) can ever match).
    /// </summary>
    Task<List<Complex>> SearchAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount);

    /// <summary>
    /// Admin-only: the same search, across every Tenant's Complexes at once. Callers MUST gate
    /// this behind an admin-only check - it deliberately bypasses the IMultiTenant filter that
    /// keeps every other Complex query scoped to the caller's own site.
    /// </summary>
    Task<List<Complex>> SearchAcrossAllTenantsAsync(
        int? cityId, int? districtId, int? neighborhoodId, string? name, int maxResultCount);

    /// <summary>
    /// Admin-only: fetches a single Complex regardless of which Tenant owns it. Callers MUST gate
    /// this behind an admin-only check, same as <see cref="SearchAcrossAllTenantsAsync"/>.
    /// </summary>
    Task<Complex> GetAcrossAllTenantsAsync(Guid id);
}
