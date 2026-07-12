using System;
using System.Threading.Tasks;
using KatKat.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace KatKat.RateLimiting;

/// <summary>
/// Enforces a distributed, per-HTTP-method rate limit keyed by the current user (or, for
/// anonymous callers, their remote IP) AND the specific action being called. Limits are read from
/// ABP Settings on every request, so an admin can tighten/loosen them at runtime without a
/// redeploy.
/// </summary>
public class RateLimitFilter : IAsyncActionFilter
{
    private readonly IRateLimitStore _rateLimitStore;
    private readonly ISettingProvider _settingProvider;
    private readonly ICurrentUser _currentUser;

    public RateLimitFilter(IRateLimitStore rateLimitStore, ISettingProvider settingProvider, ICurrentUser currentUser)
    {
        _rateLimitStore = rateLimitStore;
        _settingProvider = settingProvider;
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpMethod = context.HttpContext.Request.Method;
        var (permitSetting, windowSetting, defaultPermit, defaultWindow) = RateLimitSettingResolver.For(httpMethod);

        var permitLimit = await _settingProvider.GetAsync(permitSetting, defaultPermit);
        var windowSeconds = await _settingProvider.GetAsync(windowSetting, defaultWindow);

        var clientKey = _currentUser.Id?.ToString() ?? context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // Scoped per action (not just per HTTP method) so unrelated endpoints don't share one
        // budget - e.g. a page firing several different GETs on load, or a screen that legitimately
        // re-fetches its own data often (a live map/leaderboard), no longer starves every other
        // GET the same user makes within the same second.
        var actionKey = context.ActionDescriptor.DisplayName ?? context.ActionDescriptor.Id;
        var rateLimitKey = $"{RateLimitConsts.CacheKeyPrefix}{httpMethod}:{actionKey}:{clientKey}";

        var allowed = await _rateLimitStore.TryAcquireAsync(rateLimitKey, permitLimit, TimeSpan.FromSeconds(windowSeconds));
        if (!allowed)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);
            return;
        }

        await next();
    }
}
