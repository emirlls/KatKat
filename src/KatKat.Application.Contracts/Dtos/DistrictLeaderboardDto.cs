using System.Collections.Generic;
using KatKat.Dtos.Common;

namespace KatKat.Dtos;

/// <summary>
/// One district's own leaderboard (ranked from 1 within that district), grouped alongside every
/// other district's for the "all districts" convenience endpoint.
/// </summary>
public class DistrictLeaderboardDto
{
    public LookupDto District { get; set; } = null!;

    public List<LeaderboardDto> Entries { get; set; } = new();
}
