using System;

namespace KatKat.Dtos;

public class CreateComplexDto
{
    public string Name { get; set; } = null!;

    public int NeighborhoodId { get; set; }

    public string? Address { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime SubscriptionStartDate { get; set; }
}
