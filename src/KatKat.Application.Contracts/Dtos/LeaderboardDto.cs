using System;

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

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public decimal Score { get; set; }

    public DateTime CalculatedAt { get; set; }
}
