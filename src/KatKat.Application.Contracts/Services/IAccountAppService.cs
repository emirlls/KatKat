using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Services;

public interface IAccountAppService
{
    /// <summary>Admin-only: creates a new Manager account and its own isolated Tenant.</summary>
    Task CreateManagerAsync(CreateManagerDto input);

    /// <summary>
    /// Anonymous: resolves which Tenant a username belongs to, so the login flow can pass
    /// it along to the token endpoint before any tenant context exists. Returns null for
    /// host-level users (e.g. "admin") or unknown usernames - either way, no tenant hint is needed.
    /// </summary>
    Task<Guid?> GetTenantIdByUserNameAsync(string userName);

    /// <summary>
    /// Admin-only: every Manager across every Tenant, with their site's location if they've
    /// created one - filters are optional and AND-combined; only the most specific location
    /// filter given is applied (neighborhoodId, else districtId, else cityId).
    /// </summary>
    Task<List<ManagerListItemDto>> GetManagersAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount);

    /// <summary>Admin-only: corrects a Manager's username/email/phone.</summary>
    Task<ManagerListItemDto> UpdateManagerAsync(Guid tenantId, UpdateManagerDto input);
}
