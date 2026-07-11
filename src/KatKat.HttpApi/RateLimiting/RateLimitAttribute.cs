using Microsoft.AspNetCore.Mvc;

namespace KatKat.RateLimiting;

/// <summary>
/// Applies the Redis-backed, per-HTTP-method distributed rate limit (see <see cref="RateLimitFilter"/>)
/// to a controller or action. Limits are admin-configurable at runtime via ABP Setting Management -
/// no redeploy required.
/// </summary>
public class RateLimitAttribute : TypeFilterAttribute
{
    public RateLimitAttribute() : base(typeof(RateLimitFilter))
    {
    }
}
