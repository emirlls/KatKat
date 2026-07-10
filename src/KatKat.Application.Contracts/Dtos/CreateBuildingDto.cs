using System;

namespace KatKat.Dtos;

public class CreateBuildingDto
{
    public Guid ComplexId { get; set; }

    public string Name { get; set; } = null!;

    public int? FloorCount { get; set; }
}
