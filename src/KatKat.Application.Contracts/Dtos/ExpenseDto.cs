using System;
using KatKat.Enums;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ExpenseDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal TotalAmount { get; set; }

    public ExpenseDistributionModes DistributionModes { get; set; }

    public DateTime IssuedAt { get; set; }

    public string? ReceiptImageUrl { get; set; }
}
