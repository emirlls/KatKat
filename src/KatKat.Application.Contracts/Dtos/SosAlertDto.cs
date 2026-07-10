using System;
using KatKat.Enums;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class SosAlertDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public Guid FlatId { get; set; }

    public Guid ReporterUserId { get; set; }

    public SosStatuses Status { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }
}
