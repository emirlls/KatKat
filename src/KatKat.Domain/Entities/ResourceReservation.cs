using System;
using KatKat.Constants;
using KatKat.Resources;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// An hourly booking of a Resource. Overlap with other Confirmed reservations for the same
/// Resource is rejected by ResourceReservationManager before this is ever constructed.
/// </summary>
public class ResourceReservation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ResourceId { get; protected set; }

    public virtual Guid ReservedByUserId { get; protected set; }

    public virtual DateTime StartTime { get; protected set; }

    public virtual DateTime EndTime { get; protected set; }

    public virtual ReservationStatus Status { get; protected set; }

    protected ResourceReservation()
    {
        /* EF Core */
    }

    internal ResourceReservation(
        Guid id, Guid? tenantId, Guid resourceId, Guid reservedByUserId, DateTime startTime, DateTime endTime)
        : base(id)
    {
        TenantId = tenantId;
        ResourceId = resourceId;
        ReservedByUserId = reservedByUserId;
        SetTimeRange(startTime, endTime);
        Status = ReservationStatus.Confirmed;
    }

    private void SetTimeRange(DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationEndTimeMustBeAfterStartTime);
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Confirmed)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationMustBeConfirmedToCancel);
        }

        Status = ReservationStatus.Cancelled;
    }
}
