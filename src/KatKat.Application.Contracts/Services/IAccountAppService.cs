using System;
using System.Threading.Tasks;
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
}
