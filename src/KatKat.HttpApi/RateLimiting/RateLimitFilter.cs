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
/// anonymous callers, their remote IP). Limits are read from ABP Settings on every request, so
/// an admin can tighten/loosen them at runtime without a redeploy.
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
        var rateLimitKey = $"{RateLimitConsts.CacheKeyPrefix}{httpMethod}:{clientKey}";

        var allowed = await _rateLimitStore.TryAcquireAsync(rateLimitKey, permitLimit, TimeSpan.FromSeconds(windowSeconds));
        if (!allowed)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);
            return;
        }

        await next();
    }
}
