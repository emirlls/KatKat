namespace KatKat.Constants;

public static class FlatConsts
{
    public const string TableName = "Flats";

    public const int MaxFlatNumberLength = 16;

    /// <summary>
    /// Precision/scale used for the ShareFactor (arsa payı) decimal column: up to 999.999999.
    /// </summary>
    public const int ShareFactorPrecision = 9;

    public const int ShareFactorScale = 6;
}
