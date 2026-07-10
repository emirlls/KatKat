using System;

namespace KatKat.Dtos;

public class CreateResourceReservationDto
{
    public Guid ResourceId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}
