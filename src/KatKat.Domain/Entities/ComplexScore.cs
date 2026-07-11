using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace KatKat.Entities;

/// <summary>
/// Latest nightly-calculated KatKat Score snapshot for one Complex. Deliberately NOT
/// IMultiTenant: the district/city leaderboard is an inherently cross-tenant read (KVKK
/// privacy shield), and this table only ever holds aggregated numbers - never raw member,
/// expense, or complaint data - so there is nothing tenant-sensitive to leak by not filtering it.
/// TenantId is nullable to match Complex.TenantId (host-owned when multi-tenancy is disabled).
/// </summary>
public class ComplexScore : FullAuditedAggregateRoot<Guid>
{
    public virtual Guid ComplexId { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// Denormalized copy of Complex.Name, refreshed on every recalculation - needed so the
    /// cross-tenant leaderboard can display a name without joining back into tenant-filtered data.
    /// </summary>
    public virtual string Name { get; protected set; } = null!;

    /// <summary>
    /// Denormalized copy of the full City/District/Neighborhood hierarchy (unlike Complex, which
    /// only stores the leaf NeighborhoodId) - this is deliberately flat so district/neighborhood
    /// leaderboard queries never need a 3-table join, matching how Name/Latitude/Longitude are
    /// already denormalized here for the same reason. Refreshed on every recalculation.
    /// </summary>
    public virtual int CityId { get; protected set; }

    public virtual int DistrictId { get; protected set; }

    public virtual int NeighborhoodId { get; protected set; }

    /// <summary>Denormalized copy of Complex.Latitude/Longitude - powers the nearby leaderboard.</summary>
    public virtual decimal Latitude { get; protected set; }

    public virtual decimal Longitude { get; protected set; }

    public virtual decimal FinancialScore { get; protected set; }

    public virtual decimal SocialScore { get; protected set; }

    public virtual decimal ResolutionScore { get; protected set; }

    public virtual decimal TotalScore { get; protected set; }

    public virtual DateTime CalculatedAt { get; protected set; }

    protected ComplexScore()
    {
        /* EF Core */
    }

    internal ComplexScore(
        Guid id,
        Guid complexId,
        Guid? tenantId,
        string name,
        int cityId,
        int districtId,
        int neighborhoodId,
        decimal latitude,
        decimal longitude,
        decimal financialScore,
        decimal socialScore,
        decimal resolutionScore,
        decimal totalScore,
        DateTime calculatedAt)
        : base(id)
    {
        ComplexId = complexId;
        TenantId = tenantId;
        Update(name, cityId, districtId, neighborhoodId, latitude, longitude, financialScore, socialScore, resolutionScore, totalScore, calculatedAt);
    }

    public void Update(
        string name,
        int cityId,
        int districtId,
        int neighborhoodId,
        decimal latitude,
        decimal longitude,
        decimal financialScore,
        decimal socialScore,
        decimal resolutionScore,
        decimal totalScore,
        DateTime calculatedAt)
    {
        Name = name;
        CityId = cityId;
        DistrictId = districtId;
        NeighborhoodId = neighborhoodId;
        Latitude = latitude;
        Longitude = longitude;
        FinancialScore = financialScore;
        SocialScore = socialScore;
        ResolutionScore = resolutionScore;
        TotalScore = totalScore;
        CalculatedAt = calculatedAt;
    }
}
