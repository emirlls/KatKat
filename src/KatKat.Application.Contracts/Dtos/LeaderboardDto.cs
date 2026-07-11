using System;
using KatKat.Dtos.Common;

namespace KatKat.Dtos;

/// <summary>
/// One row of the district/city leaderboard. Deliberately exposes only aggregated,
/// non-sensitive fields (KVKK privacy shield) - never raw member/expense/complaint data.
/// </summary>
public class LeaderboardDto
{
    public int Rank { get; set; }

    public Guid ComplexId { get; set; }

    public string ComplexName { get; set; } = null!;

    public LookupDto City { get; set; } = null!;

    public LookupDto District { get; set; } = null!;

    public LookupDto Neighborhood { get; set; } = null!;

    /// <summary>Always populated - lets both district-based and nearby-based leaderboards render on a map.</summary>
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal Score { get; set; }

    public DateTime CalculatedAt { get; set; }

    /// <summary>Distance (km) from the query point - populated only by the nearby-radius leaderboard, null otherwise.</summary>
    public double? DistanceKm { get; set; }
}
