using System;
using KatKat.Constants;
using KatKat.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A shared expense raised against a Complex (Smart Billing) - the manager photographs a
/// receipt, enters the total, and the system splits it into per-flat ExpenseShares.
/// </summary>
public class Expense : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    public virtual string Title { get; protected set; } = null!;

    public virtual string? Description { get; protected set; }

    public virtual decimal TotalAmount { get; protected set; }

    public virtual ExpenseDistributionModes DistributionModes { get; protected set; }

    public virtual DateTime IssuedAt { get; protected set; }

    public virtual string? ReceiptImageUrl { get; protected set; }

    protected Expense()
    {
        /* EF Core */
    }

    internal Expense(
        Guid id,
        Guid? tenantId,
        Guid complexId,
        string title,
        string? description,
        decimal totalAmount,
        ExpenseDistributionModes distributionModes,
        DateTime issuedAt,
        string? receiptImageUrl)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        SetTitle(title);
        SetDescription(description);
        SetTotalAmount(totalAmount);
        DistributionModes = distributionModes;
        IssuedAt = issuedAt;
        SetReceiptImageUrl(receiptImageUrl);
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), ExpenseConsts.MaxTitleLength);
    }

    public void SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(description), ExpenseConsts.MaxDescriptionLength);
    }

    public void SetReceiptImageUrl(string? receiptImageUrl)
    {
        ReceiptImageUrl = Check.Length(receiptImageUrl, nameof(receiptImageUrl), ExpenseConsts.MaxReceiptImageUrlLength);
    }

    public void SetTotalAmount(decimal totalAmount)
    {
        if (totalAmount <= 0)
        {
            throw new BusinessException(KatKatErrorCodes.ExpenseTotalAmountMustBePositive);
        }

        TotalAmount = totalAmount;
    }
}
