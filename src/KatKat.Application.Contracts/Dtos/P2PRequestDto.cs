using System;
using KatKat.P2PRequests;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class P2PRequestDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public Guid RequesterUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? NeededUntil { get; set; }

    public P2PRequestStatus Status { get; set; }

    public Guid? FulfilledByUserId { get; set; }

    public DateTime? FulfilledAt { get; set; }
}
