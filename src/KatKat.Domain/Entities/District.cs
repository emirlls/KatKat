using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace KatKat.Entities;

/// <summary>
/// A district (ilçe) within a City. Only holds a reference to its City by Id - no navigation
/// property, to keep aggregates independently loadable (same convention as Building.ComplexId).
/// </summary>
public class District : FullAuditedAggregateRoot<int>, IHasDisplayName
{
    public virtual int CityId { get; protected set; }

    public virtual string Name { get; protected set; } = null!;

    protected District()
    {
        /* EF Core */
    }

    internal District(int cityId, string name)
    {
        CityId = cityId;
        SetName(name);
    }

    public void SetCityId(int cityId)
    {
        CityId = cityId;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.MaxNameLength);
    }
}
