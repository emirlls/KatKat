using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ExpenseShareDto : FullAuditedEntityDto<Guid>
{
    public Guid ExpenseId { get; set; }

    public Guid FlatId { get; set; }

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? PaidAt { get; set; }
}
