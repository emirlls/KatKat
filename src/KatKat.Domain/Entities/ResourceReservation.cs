using System;
using KatKat.Constants;
using KatKat.Resources;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A booking of a Resource. Starts life as <see cref="ReservationStatus.Pending"/> and only blocks
/// the slot once a manager approves it (-> Confirmed). Overlap with other Confirmed reservations for
/// the same Resource is rejected both at request time and again at approval time.
/// </summary>
public class ResourceReservation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ResourceId { get; protected set; }

    /// <summary>The reserver's own Flat within the Resource's Complex, resolved server-side at
    /// creation time - null if the reserver somehow has no Flat there. Lets everyone see which
    /// Building/Flat a booking came from.</summary>
    public virtual Guid? FlatId { get; protected set; }

    public virtual Guid ReservedByUserId { get; protected set; }

    public virtual DateTime StartTime { get; protected set; }

    public virtual DateTime EndTime { get; protected set; }

    public virtual ReservationStatus Status { get; protected set; }

    /// <summary>The manager's reason for declining - only ever set by <see cref="Reject"/>.</summary>
    public virtual string? RejectionReason { get; protected set; }

    protected ResourceReservation()
    {
        /* EF Core */
    }

    internal ResourceReservation(
        Guid id, Guid? tenantId, Guid resourceId, Guid? flatId, Guid reservedByUserId, DateTime startTime, DateTime endTime)
        : base(id)
    {
        TenantId = tenantId;
        ResourceId = resourceId;
        FlatId = flatId;
        ReservedByUserId = reservedByUserId;
        SetTimeRange(startTime, endTime);
        Status = ReservationStatus.Pending;
    }

    private void SetTimeRange(DateTime startTime, DateTime endTime)
    {
        // A same-instant range (start == end) is allowed - the UI lets a resident book a single
        // day/slot where both bounds land on the same value; only end strictly before start is invalid.
        if (endTime < startTime)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationEndTimeMustBeAfterStartTime);
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>Manager decision: accept the pending request. Overlap is re-checked by the manager first.</summary>
    public void Approve()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationMustBePendingToDecide);
        }

        Status = ReservationStatus.Confirmed;
    }

    /// <summary>Manager decision: decline the pending request, with a reason shown to the reserver.</summary>
    public void Reject(string reason)
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationMustBePendingToDecide);
        }

        Status = ReservationStatus.Rejected;
        RejectionReason = reason;
    }

    public void Cancel()
    {
        // The reserver may withdraw a booking while it is still pending or after it was confirmed;
        // an already cancelled/rejected booking cannot be cancelled again.
        if (Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
        {
            throw new BusinessException(KatKatErrorCodes.ReservationCannotBeCancelled);
        }

        Status = ReservationStatus.Cancelled;
    }
}
