using System;
using KatKat.Enums;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class SosAlertDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public Guid FlatId { get; set; }

    /// <summary>Denormalized from Flat.FlatNumber - a raw Guid means nothing to a person reading the SOS floor plan.</summary>
    public string FlatNumber { get; set; } = null!;

    /// <summary>Denormalized from Building.Name (via Flat.BuildingId).</summary>
    public string BuildingName { get; set; } = null!;

    public Guid ReporterUserId { get; set; }

    public SosStatuses Status { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }
}
