using System;
using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A block within a Complex (e.g. "A Blok"). Only holds a reference to its Complex by Id -
/// no navigation property, to keep aggregates independently loadable.
/// </summary>
public class Building : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    public virtual string Name { get; protected set; } = null!;

    public virtual int? FloorCount { get; protected set; }

    protected Building()
    {
        /* EF Core */
    }

    internal Building(
        Guid id,
        Guid? tenantId,
        Guid complexId,
        string name,
        int? floorCount)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        SetName(name);
        SetFloorCount(floorCount);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), BuildingConsts.MaxNameLength);
    }

    public void SetFloorCount(int? floorCount)
    {
        if (floorCount.HasValue && floorCount.Value <= 0)
        {
            throw new BusinessException(KatKatErrorCodes.BuildingFloorCountMustBePositive);
        }

        FloorCount = floorCount;
    }
}
