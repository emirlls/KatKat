using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ExpenseShareDto : FullAuditedEntityDto<Guid>
{
    public Guid ExpenseId { get; set; }

    public Guid FlatId { get; set; }

    /// <summary>Denormalized from Flat.FlatNumber - a raw Guid means nothing to a person reading a bill.</summary>
    public string FlatNumber { get; set; } = null!;

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? PaidAt { get; set; }
}
