using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.FlatMembers;

/// <summary>
/// Relational cross-entity linking an ABP IdentityUser (by Id only - no cross-module
/// navigation) to a Flat, carrying that user's role/state within the flat.
/// </summary>
public class FlatMember : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid FlatId { get; protected set; }

    public virtual Guid UserId { get; protected set; }

    public virtual FlatMemberRole Role { get; protected set; }

    protected FlatMember()
    {
        /* EF Core */
    }

    internal FlatMember(
        Guid id,
        Guid? tenantId,
        Guid flatId,
        Guid userId,
        FlatMemberRole role)
        : base(id)
    {
        TenantId = tenantId;
        FlatId = flatId;
        UserId = userId;
        Role = role;
    }

    /// <summary>
    /// Transitions an UnverifiedResident to Resident once a Manager verifies them.
    /// </summary>
    public void Approve()
    {
        if (Role != FlatMemberRole.UnverifiedResident)
        {
            throw new BusinessException(KatKatErrorCodes.FlatMemberMustBeUnverifiedToApprove);
        }

        Role = FlatMemberRole.Resident;
    }

    public void PromoteToManager()
    {
        if (Role == FlatMemberRole.UnverifiedResident)
        {
            throw new BusinessException(KatKatErrorCodes.FlatMemberCannotBePromotedWhileUnverified);
        }

        Role = FlatMemberRole.Manager;
    }
}
