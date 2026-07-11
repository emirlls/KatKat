using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace KatKat.Entities;

/// <summary>
/// A neighborhood (mahalle) within a District. Only holds a reference to its District by Id -
/// no navigation property, same convention as every other FK in this codebase.
/// </summary>
public class Neighborhood : FullAuditedAggregateRoot<int>, IHasDisplayName
{
    public virtual int DistrictId { get; protected set; }

    public virtual string Name { get; protected set; } = null!;

    protected Neighborhood()
    {
        /* EF Core */
    }

    internal Neighborhood(int districtId, string name)
    {
        DistrictId = districtId;
        SetName(name);
    }

    public void SetDistrictId(int districtId)
    {
        DistrictId = districtId;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), NeighborhoodConsts.MaxNameLength);
    }
}
