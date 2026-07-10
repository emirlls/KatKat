namespace KatKat.Constants;

/// <summary>
/// Cross-cutting constants shared by more than one layer (e.g. a default parameter value that
/// must match between an app service interface, its implementation, and its controller).
/// </summary>
public static class KatKatConsts
{
    public const int DefaultLeaderboardMaxResultCount = 20;

    /// <summary>Precision/scale shared by every 0-100 KatKat Score column (e.g. 100.00).</summary>
    public const int ScorePrecision = 5;
    public const int ScoreScale = 2;

    /// <summary>Precision/scale shared by every money amount column (e.g. 999999999.99).</summary>
    public const int AmountPrecision = 11;
    public const int AmountScale = 2;
}
