using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// Per-user opt-in/opt-out notification settings. Official notices can never be muted, but
/// residents may opt out of ad-hoc neighbor P2P request pushes.
/// </summary>
public class UserPreference : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid UserId { get; protected set; }

    public virtual bool ReceiveNeighborRequestNotifications { get; protected set; }

    protected UserPreference()
    {
        /* EF Core */
    }

    internal UserPreference(Guid id, Guid? tenantId, Guid userId)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        ReceiveNeighborRequestNotifications = true;
    }

    public void SetNeighborRequestNotifications(bool enabled)
    {
        ReceiveNeighborRequestNotifications = enabled;
    }
}
