using System;
using System.Threading.Tasks;

namespace KatKat.RateLimiting;

/// <summary>
/// A distributed counter store used to enforce per-key rate limits. Implemented against Redis
/// (see <c>RedisRateLimitStore</c> in the host project) so limits are shared across every
/// instance of the app, not just enforced per-process.
/// </summary>
public interface IRateLimitStore
{
    /// <summary>
    /// Atomically increments the counter for <paramref name="key"/> and returns whether the
    /// caller is still within <paramref name="permitLimit"/> for the current <paramref name="window"/>.
    /// </summary>
    Task<bool> TryAcquireAsync(string key, int permitLimit, TimeSpan window);
}
