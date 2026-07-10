using System;
using KatKat.Resources;

namespace KatKat.Dtos;

public class CreateResourceDto
{
    public Guid ComplexId { get; set; }

    public string Name { get; set; } = null!;

    public ResourceType Type { get; set; }
}
