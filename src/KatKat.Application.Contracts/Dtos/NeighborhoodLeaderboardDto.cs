using System.Collections.Generic;
using KatKat.Dtos.Common;

namespace KatKat.Dtos;

/// <summary>
/// One neighborhood's own leaderboard (ranked from 1 within that neighborhood), grouped
/// alongside every other neighborhood's for the "all neighborhoods" convenience endpoint.
/// Paired with its District since neighborhood names are not guaranteed unique across districts.
/// </summary>
public class NeighborhoodLeaderboardDto
{
    public LookupDto District { get; set; } = null!;

    public LookupDto Neighborhood { get; set; } = null!;

    public List<LeaderboardDto> Entries { get; set; } = new();
}
