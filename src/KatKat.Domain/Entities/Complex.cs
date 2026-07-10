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

    public virtual string City { get; protected set; } = null!;

    public virtual string District { get; protected set; } = null!;

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
        string city,
        string district,
        string? address,
        decimal latitude,
        decimal longitude,
        DateTime subscriptionStartDate)
        : base(id)
    {
        TenantId = tenantId;
        SetName(name);
        SetCity(city);
        SetDistrict(district);
        SetAddress(address);
        SetLocation(latitude, longitude);
        SubscriptionStartDate = subscriptionStartDate;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ComplexConsts.MaxNameLength);
    }

    public void SetCity(string city)
    {
        City = Check.NotNullOrWhiteSpace(city, nameof(city), ComplexConsts.MaxCityLength);
    }

    public void SetDistrict(string district)
    {
        District = Check.NotNullOrWhiteSpace(district, nameof(district), ComplexConsts.MaxDistrictLength);
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
