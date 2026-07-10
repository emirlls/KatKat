using System;

namespace KatKat.Dtos;

public class CreateFlatDto
{
    public Guid BuildingId { get; set; }

    public string FlatNumber { get; set; } = null!;

    public int? FloorNumber { get; set; }

    public decimal ShareFactor { get; set; }
}
