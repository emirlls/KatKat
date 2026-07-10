using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Flats;

/// <summary>
/// An individual apartment. ShareFactor is the land-share ratio (arsa payı) used for
/// proportional expense distribution in Smart Billing.
/// </summary>
public class Flat : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid BuildingId { get; protected set; }

    public virtual string FlatNumber { get; protected set; } = null!;

    public virtual int? FloorNumber { get; protected set; }

    public virtual decimal ShareFactor { get; protected set; }

    protected Flat()
    {
        /* EF Core */
    }

    internal Flat(
        Guid id,
        Guid? tenantId,
        Guid buildingId,
        string flatNumber,
        int? floorNumber,
        decimal shareFactor)
        : base(id)
    {
        TenantId = tenantId;
        BuildingId = buildingId;
        SetFlatNumber(flatNumber);
        FloorNumber = floorNumber;
        SetShareFactor(shareFactor);
    }

    public void SetFlatNumber(string flatNumber)
    {
        FlatNumber = Check.NotNullOrWhiteSpace(flatNumber, nameof(flatNumber), FlatConsts.MaxFlatNumberLength);
    }

    public void SetShareFactor(decimal shareFactor)
    {
        if (shareFactor <= 0)
        {
            throw new BusinessException(KatKatErrorCodes.FlatShareFactorMustBeGreaterThanZero);
        }

        ShareFactor = shareFactor;
    }
}
