using System;
using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// The root tenant entity: one Complex (site/apartment group) maps to exactly one ABP tenant.
/// </summary>
public class Complex : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual string Name { get; protected set; } = null!;

    /// <summary>
    /// Leaf of the City -> District -> Neighborhood lookup hierarchy. City/District are derivable
    /// through this Id, so only the leaf is stored here (normalized, single source of truth).
    /// </summary>
    public virtual int NeighborhoodId { get; protected set; }

    public virtual string? Address { get; protected set; }

    public virtual decimal Latitude { get; protected set; }

    public virtual decimal Longitude { get; protected set; }

    public virtual DateTime SubscriptionStartDate { get; protected set; }

    public virtual DateTime? SubscriptionEndDate { get; protected set; }

    protected Complex()
    {
        /* EF Core */
    }

    internal Complex(
        Guid id,
        Guid tenantId,
        string name,
        int neighborhoodId,
        string? address,
        decimal latitude,
        decimal longitude,
        DateTime subscriptionStartDate)
        : base(id)
    {
        TenantId = tenantId;
        SetName(name);
        SetNeighborhood(neighborhoodId);
        SetAddress(address);
        SetLocation(latitude, longitude);
        SubscriptionStartDate = subscriptionStartDate;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ComplexConsts.MaxNameLength);
    }

    public void SetNeighborhood(int neighborhoodId)
    {
        NeighborhoodId = neighborhoodId;
    }

    public void SetAddress(string? address)
    {
        Address = Check.Length(address, nameof(address), ComplexConsts.MaxAddressLength);
    }

    public void SetLocation(decimal latitude, decimal longitude)
    {
        if (latitude < GeoConsts.MinLatitude || latitude > GeoConsts.MaxLatitude)
        {
            throw new BusinessException(KatKatErrorCodes.ComplexLatitudeOutOfRange)
                .WithData("minLatitude", GeoConsts.MinLatitude)
                .WithData("maxLatitude", GeoConsts.MaxLatitude);
        }

        if (longitude < GeoConsts.MinLongitude || longitude > GeoConsts.MaxLongitude)
        {
            throw new BusinessException(KatKatErrorCodes.ComplexLongitudeOutOfRange)
                .WithData("minLongitude", GeoConsts.MinLongitude)
                .WithData("maxLongitude", GeoConsts.MaxLongitude);
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public void ExtendSubscription(DateTime newEndDate)
    {
        if (SubscriptionEndDate.HasValue && newEndDate <= SubscriptionEndDate.Value)
        {
            throw new BusinessException(KatKatErrorCodes.SubscriptionEndDateMustBeLaterThanCurrentEndDate);
        }

        SubscriptionEndDate = newEndDate;
    }
}
