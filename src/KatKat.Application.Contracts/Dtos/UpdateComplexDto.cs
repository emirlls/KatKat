namespace KatKat.Dtos;

public class UpdateComplexDto
{
    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string? Address { get; set; }
}
