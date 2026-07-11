namespace KatKat.Entities;

/// <summary>
/// Marker for entities with a display Name, so generic lookup-resolution code (see
/// LocationLookupResolver) can batch-fetch a {Id, Name} projection without per-entity code.
/// Deliberately doesn't redeclare Id - implementers already get that from IEntity{TKey}, and
/// redeclaring it here would make e.Id ambiguous wherever both constraints are combined.
/// </summary>
public interface IHasDisplayName
{
    string Name { get; }
}
