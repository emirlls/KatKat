using System;
using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// One flat's calculated portion of an Expense. Payment speed across a Complex's
/// ExpenseShares feeds the Financial metric (40%) of the KatKat Score.
/// </summary>
public class ExpenseShare : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ExpenseId { get; protected set; }

    public virtual Guid FlatId { get; protected set; }

    public virtual decimal Amount { get; protected set; }

    public virtual bool IsPaid { get; protected set; }

    public virtual DateTime? PaidAt { get; protected set; }

    protected ExpenseShare()
    {
        /* EF Core */
    }

    internal ExpenseShare(Guid id, Guid? tenantId, Guid expenseId, Guid flatId, decimal amount)
        : base(id)
    {
        TenantId = tenantId;
        ExpenseId = expenseId;
        FlatId = flatId;
        Amount = amount;
        IsPaid = false;
    }

    public void MarkAsPaid()
    {
        if (IsPaid)
        {
            throw new BusinessException(KatKatErrorCodes.ExpenseShareAlreadyPaid);
        }

        IsPaid = true;
        PaidAt = DateTime.UtcNow;
    }
}
