namespace KatKat.Constants;

/// <summary>
/// Shared geographic bounds/precision used by every entity that stores a coordinate
/// (Complex today, ComplexScore's denormalized copy).
/// </summary>
public static class GeoConsts
{
    public const decimal MinLatitude = -90m;
    public const decimal MaxLatitude = 90m;
    public const decimal MinLongitude = -180m;
    public const decimal MaxLongitude = 180m;

    /// <summary>Precision/scale for lat/lng decimal columns: up to 999.999999.</summary>
    public const int CoordinatePrecision = 9;
    public const int CoordinateScale = 6;
}
