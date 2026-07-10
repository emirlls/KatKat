using System;
using KatKat.Resources;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ResourceReservationDto : FullAuditedEntityDto<Guid>
{
    public Guid ResourceId { get; set; }

    public Guid ReservedByUserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ReservationStatus Status { get; set; }
}
