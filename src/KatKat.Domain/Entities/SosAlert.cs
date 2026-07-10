using System;
using KatKat.Constants;
using KatKat.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A single "Güvendeyim" / "Yardım Lazım" button press during a crisis. The manager's digital
/// floor plan is built from the latest SosAlert per Flat in a Complex.
/// </summary>
public class SosAlert : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    public virtual Guid FlatId { get; protected set; }

    public virtual Guid ReporterUserId { get; protected set; }

    public virtual SosStatuses Status { get; protected set; }

    public virtual DateTime? ResolvedAt { get; protected set; }

    public virtual Guid? ResolvedByUserId { get; protected set; }

    protected SosAlert()
    {
        /* EF Core */
    }

    internal SosAlert(Guid id, Guid? tenantId, Guid complexId, Guid flatId, Guid reporterUserId, SosStatuses status)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        FlatId = flatId;
        ReporterUserId = reporterUserId;
        Status = status;
    }

    /// <summary>Manager acknowledges help has reached a HelpNeeded alert.</summary>
    public void Resolve(Guid resolvedByUserId)
    {
        if (Status != SosStatuses.HelpNeeded)
        {
            throw new BusinessException(KatKatErrorCodes.SosAlertMustBeHelpNeededToResolve);
        }

        ResolvedAt = DateTime.UtcNow;
        ResolvedByUserId = resolvedByUserId;
    }
}
