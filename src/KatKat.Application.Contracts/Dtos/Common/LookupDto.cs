namespace KatKat.Dtos.Common;

/// <summary>
/// Reusable {Id, Name} shape for nested location references (City/District/Neighborhood) in
/// other DTOs, so callers don't need a full round-trip to resolve a display name from an id.
/// </summary>
public class LookupDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
