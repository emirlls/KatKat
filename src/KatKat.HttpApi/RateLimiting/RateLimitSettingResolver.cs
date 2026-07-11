using System;
using System.Collections.Generic;
using KatKat.Constants;
using KatKat.Settings;

namespace KatKat.RateLimiting;

/// <summary>
/// Maps an HTTP method to the ABP Setting names (and in-code defaults) that govern its rate
/// limit. Unrecognized methods fall back to the POST limits, since POST is the strictest
/// (mutating) default and the safest choice when a method can't be matched.
/// </summary>
internal static class RateLimitSettingResolver
{
    private static readonly Dictionary<string, (string PermitSetting, string WindowSetting, int DefaultPermit, int DefaultWindow)> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["GET"] = (KatKatSettings.RateLimiting.GetPermitLimit, KatKatSettings.RateLimiting.GetWindowSeconds, RateLimitConsts.DefaultGetPermitLimit, RateLimitConsts.DefaultGetWindowSeconds),
            ["POST"] = (KatKatSettings.RateLimiting.PostPermitLimit, KatKatSettings.RateLimiting.PostWindowSeconds, RateLimitConsts.DefaultPostPermitLimit, RateLimitConsts.DefaultPostWindowSeconds),
            ["PUT"] = (KatKatSettings.RateLimiting.PutPermitLimit, KatKatSettings.RateLimiting.PutWindowSeconds, RateLimitConsts.DefaultPutPermitLimit, RateLimitConsts.DefaultPutWindowSeconds),
            ["DELETE"] = (KatKatSettings.RateLimiting.DeletePermitLimit, KatKatSettings.RateLimiting.DeleteWindowSeconds, RateLimitConsts.DefaultDeletePermitLimit, RateLimitConsts.DefaultDeleteWindowSeconds),
            ["PATCH"] = (KatKatSettings.RateLimiting.PatchPermitLimit, KatKatSettings.RateLimiting.PatchWindowSeconds, RateLimitConsts.DefaultPatchPermitLimit, RateLimitConsts.DefaultPatchWindowSeconds),
        };

    public static (string PermitSetting, string WindowSetting, int DefaultPermit, int DefaultWindow) For(string httpMethod) =>
        Map.TryGetValue(httpMethod, out var entry) ? entry : Map["POST"];
}
