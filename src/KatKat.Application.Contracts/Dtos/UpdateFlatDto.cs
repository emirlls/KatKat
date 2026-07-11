namespace KatKat.Dtos;

public class UpdateFlatDto
{
    public string FlatNumber { get; set; } = null!;

    public int? FloorNumber { get; set; }

    public decimal ShareFactor { get; set; }
}
