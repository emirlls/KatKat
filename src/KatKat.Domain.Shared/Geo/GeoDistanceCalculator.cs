using System;
using KatKat.Constants;

namespace KatKat.Geo;

/// <summary>
/// Pure, dependency-free geographic distance math shared by the repository (bounding-box
/// prefilter) and any layer that needs an exact distance - lives in Domain.Shared since it has
/// zero framework dependencies and is referenced from both the Domain and EntityFrameworkCore
/// projects.
/// </summary>
public static class GeoDistanceCalculator
{
    /// <summary>Mean Earth radius in kilometers, used by the Haversine formula.</summary>
    public const double EarthRadiusKm = 6371.0;

    /// <summary>Approximate kilometers per degree of latitude (constant everywhere on Earth).</summary>
    public const double KmPerDegreeLatitude = 111.0;

    /// <summary>
    /// Below this many km per degree of longitude (i.e. extremely close to a pole), the
    /// bounding-box longitude delta is clamped to the full valid range instead of blowing up.
    /// </summary>
    public const double MinKmPerDegreeLongitudeBeforeClamping = 1.0;

    private const double DegreesToRadiansFactor = Math.PI / 180.0;

    /// <summary>Great-circle distance between two coordinates in kilometers (Haversine formula).</summary>
    public static double CalculateDistanceKm(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
    {
        var lat1Rad = (double)latitude1 * DegreesToRadiansFactor;
        var lat2Rad = (double)latitude2 * DegreesToRadiansFactor;
        var deltaLatRad = (double)(latitude2 - latitude1) * DegreesToRadiansFactor;
        var deltaLngRad = (double)(longitude2 - longitude1) * DegreesToRadiansFactor;

        var a = (Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2)) +
                (Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                 Math.Sin(deltaLngRad / 2) * Math.Sin(deltaLngRad / 2));

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Approximate latitude/longitude deltas covering a given radius around a center point, for
    /// a cheap SQL bounding-box prefilter that runs before the exact Haversine distance check.
    /// </summary>
    public static (decimal LatitudeDelta, decimal LongitudeDelta) CalculateBoundingBoxDelta(decimal centerLatitude, double radiusKm)
    {
        var latitudeDelta = (decimal)(radiusKm / KmPerDegreeLatitude);

        var centerLatitudeRad = (double)centerLatitude * DegreesToRadiansFactor;
        var kmPerDegreeLongitudeAtLatitude = KmPerDegreeLatitude * Math.Cos(centerLatitudeRad);

        var longitudeDelta = kmPerDegreeLongitudeAtLatitude > MinKmPerDegreeLongitudeBeforeClamping
            ? (decimal)(radiusKm / kmPerDegreeLongitudeAtLatitude)
            : GeoConsts.MaxLongitude - GeoConsts.MinLongitude;

        return (latitudeDelta, longitudeDelta);
    }
}
