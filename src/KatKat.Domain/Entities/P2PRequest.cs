using System;
using KatKat.Constants;
using KatKat.P2PRequests;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A neighbor-to-neighbor help/item request ("Komşu İhtiyaç Hub'ı"). Fulfilled requests feed
/// the Social metric (35%) of the KatKat Score.
/// </summary>
public class P2PRequest : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    /// <summary>The requester's own Flat within this Complex, resolved server-side at creation
    /// time - null if the requester somehow has no Flat here (e.g. an edge case, not the normal
    /// path). Lets everyone see which Building/Flat a request came from.</summary>
    public virtual Guid? FlatId { get; protected set; }

    public virtual Guid RequesterUserId { get; protected set; }

    public virtual string Title { get; protected set; } = null!;

    public virtual string? Description { get; protected set; }

    public virtual DateTime? NeededUntil { get; protected set; }

    public virtual P2PRequestStatus Status { get; protected set; }

    public virtual Guid? FulfilledByUserId { get; protected set; }

    public virtual DateTime? FulfilledAt { get; protected set; }

    protected P2PRequest()
    {
        /* EF Core */
    }

    internal P2PRequest(
        Guid id,
        Guid? tenantId,
        Guid complexId,
        Guid? flatId,
        Guid requesterUserId,
        string title,
        string? description,
        DateTime? neededUntil)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        FlatId = flatId;
        RequesterUserId = requesterUserId;
        SetTitle(title);
        SetDescription(description);
        NeededUntil = neededUntil;
        Status = P2PRequestStatus.Open;
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), P2PRequestConsts.MaxTitleLength);
    }

    public void SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(description), P2PRequestConsts.MaxDescriptionLength);
    }

    public void Fulfill(Guid fulfilledByUserId)
    {
        if (Status != P2PRequestStatus.Open)
        {
            throw new BusinessException(KatKatErrorCodes.P2PRequestMustBeOpenToFulfill);
        }

        if (fulfilledByUserId == RequesterUserId)
        {
            throw new BusinessException(KatKatErrorCodes.P2PRequestCannotBeFulfilledByRequester);
        }

        Status = P2PRequestStatus.Fulfilled;
        FulfilledByUserId = fulfilledByUserId;
        FulfilledAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != P2PRequestStatus.Open)
        {
            throw new BusinessException(KatKatErrorCodes.P2PRequestMustBeOpenToCancel);
        }

        Status = P2PRequestStatus.Cancelled;
    }
}
