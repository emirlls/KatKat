using System;
using KatKat.Resources;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A reservable shared resource within a Complex - a guest parking slot or a common area
/// (çardak/barbekü, oyun odası). Both share the same reservation/overlap rules, hence one entity.
/// </summary>
public class Resource : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    public virtual string Name { get; protected set; } = null!;

    public virtual ResourceType Type { get; protected set; }

    protected Resource()
    {
        /* EF Core */
    }

    internal Resource(Guid id, Guid? tenantId, Guid complexId, string name, ResourceType type)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        SetName(name);
        Type = type;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ResourceConsts.MaxNameLength);
    }
}
