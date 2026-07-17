using System;
using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A one-time, expiring code a Manager generates for a specific Flat so a resident can create
/// their own login without ever self-registering - the resident enters this code plus their own
/// name/email/phone/password to redeem it (see AccountAppService/ResidentInvitationAppService).
/// </summary>
public class ResidentInvitation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid FlatId { get; protected set; }

    public virtual string Code { get; protected set; } = null!;

    public virtual Guid CreatedByUserId { get; protected set; }

    public virtual DateTime ExpiresAt { get; protected set; }

    public virtual DateTime? RedeemedAt { get; protected set; }

    public virtual Guid? RedeemedByUserId { get; protected set; }

    protected ResidentInvitation()
    {
        /* EF Core */
    }

    internal ResidentInvitation(
        Guid id,
        Guid? tenantId,
        Guid flatId,
        string code,
        Guid createdByUserId,
        DateTime expiresAt)
        : base(id)
    {
        TenantId = tenantId;
        FlatId = flatId;
        Code = code;
        CreatedByUserId = createdByUserId;
        ExpiresAt = expiresAt;
    }

    /// <summary>Called once a resident successfully creates their account from this invitation's code.</summary>
    public void MarkRedeemed(Guid redeemedByUserId, DateTime now)
    {
        if (RedeemedAt != null)
        {
            throw new BusinessException(KatKatErrorCodes.ResidentInvitationAlreadyUsed);
        }

        if (now > ExpiresAt)
        {
            throw new BusinessException(KatKatErrorCodes.ResidentInvitationExpired);
        }

        RedeemedAt = now;
        RedeemedByUserId = redeemedByUserId;
    }
}
