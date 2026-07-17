namespace KatKat.Constants;

/// <summary>
/// Cross-cutting constants shared by more than one layer (e.g. a default parameter value that
/// must match between an app service interface, its implementation, and its controller).
/// </summary>
public static class KatKatConsts
{
    public const int DefaultLeaderboardMaxResultCount = 20;

    /// <summary>Default page size for Complex/Flat/etc search-by-filter endpoints.</summary>
    public const int DefaultSearchMaxResultCount = 20;

    /// <summary>Precision/scale shared by every 0-100 KatKat Score column (e.g. 100.00).</summary>
    public const int ScorePrecision = 5;
    public const int ScoreScale = 2;

    /// <summary>Precision/scale shared by every money amount column (e.g. 999999999.99).</summary>
    public const int AmountPrecision = 11;
    public const int AmountScale = 2;

    /// <summary>
    /// Default radius (km) for the nearby/map leaderboard, used as the seed value for the
    /// admin-configurable <c>KatKatSettings.NearbyLeaderboardRadiusKm</c> ABP Setting, and as the
    /// in-code fallback if that setting is ever missing.
    /// </summary>
    public const double DefaultNearbyLeaderboardRadiusKm = 5.0;

    /// <summary>
    /// Number of decimal places latitude/longitude are rounded to when building the nearby-leaderboard
    /// cache key, so that near-identical query points share a cache entry instead of each unique
    /// coordinate producing its own (unbounded-cardinality) key.
    /// </summary>
    public const int NearbyLeaderboardCacheKeyCoordinateRoundingDecimals = 2;
}
