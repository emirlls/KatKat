using System;

namespace KatKat.Dtos;

/// <summary>
/// Admin-only cross-tenant view of a Complex - the same fields as <see cref="ComplexDto"/>, plus
/// the owning Tenant id, for the admin "Tüm Siteler" search screen.
/// </summary>
public class AdminComplexListItemDto
{
    public ComplexDto Complex { get; set; } = null!;

    public Guid? TenantId { get; set; }
}
