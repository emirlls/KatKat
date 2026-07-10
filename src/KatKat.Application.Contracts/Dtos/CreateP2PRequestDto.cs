using System;

namespace KatKat.Dtos;

public class CreateP2PRequestDto
{
    public Guid ComplexId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? NeededUntil { get; set; }
}
