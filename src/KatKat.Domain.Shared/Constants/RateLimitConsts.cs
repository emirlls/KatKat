namespace KatKat.Constants;

/// <summary>
/// Default per-HTTP-method distributed rate-limit values, used as the seed values for the
/// admin-configurable <c>KatKatSettings.RateLimiting</c> ABP Settings, and as the in-code
/// fallback if a setting is ever missing.
/// </summary>
public static class RateLimitConsts
{
    public const string CacheKeyPrefix = "KatKat:RateLimit:";

    public const int DefaultGetPermitLimit = 3;
    public const int DefaultGetWindowSeconds = 1;
    public const int DefaultPostPermitLimit = 1;
    public const int DefaultPostWindowSeconds = 1;
    public const int DefaultPutPermitLimit = 1;
    public const int DefaultPutWindowSeconds = 1;
    public const int DefaultDeletePermitLimit = 1;
    public const int DefaultDeleteWindowSeconds = 1;
    public const int DefaultPatchPermitLimit = 1;
    public const int DefaultPatchWindowSeconds = 1;
}
