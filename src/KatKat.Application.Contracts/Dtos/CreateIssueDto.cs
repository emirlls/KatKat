using System;

namespace KatKat.Dtos;

public class CreateIssueDto
{
    public Guid ComplexId { get; set; }

    /// <summary>Which Building the fault is in (e.g. an elevator) - omit for a complex-wide issue.</summary>
    public Guid? BuildingId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoUrl { get; set; }
}
