using System;

namespace KatKat.Dtos;

public class CreateIssueDto
{
    public Guid ComplexId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoUrl { get; set; }
}
