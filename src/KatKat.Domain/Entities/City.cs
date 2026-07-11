using KatKat.Constants;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace KatKat.Entities;

/// <summary>
/// A Turkish province (il). Shared reference data used by every tenant - not IMultiTenant,
/// since geography isn't owned by any one Complex.
/// </summary>
public class City : FullAuditedAggregateRoot<int>, IHasDisplayName
{
    public virtual string Name { get; protected set; } = null!;

    protected City()
    {
        /* EF Core */
    }

    internal City(string name)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CityConsts.MaxNameLength);
    }
}
