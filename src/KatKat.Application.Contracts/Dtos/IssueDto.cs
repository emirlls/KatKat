using System;
using KatKat.Enums;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class IssueDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public Guid? BuildingId { get; set; }

    /// <summary>Denormalized from Building.Name - a raw Guid means nothing to a person reading the Issues board.</summary>
    public string? BuildingName { get; set; }

    public Guid ReporterUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoUrl { get; set; }

    public IssueStatuses Statuses { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }
}
