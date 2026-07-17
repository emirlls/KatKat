using System;
using KatKat.Dtos.Common;

namespace KatKat.Dtos;

/// <summary>
/// Admin-only cross-tenant view of a Manager: their login plus (if they've created one yet)
/// their site's name/location, for the admin "Yöneticiler" directory's search/list/edit UI.
/// PhoneNumber (KVKK-sensitive) is included here deliberately - this DTO must never be returned
/// by anything reachable by a Manager/Resident, only the admin-gated directory endpoints.
/// </summary>
public class ManagerListItemDto
{
    /// <summary>The Manager's own IdentityUser id.</summary>
    public Guid Id { get; set; }

    /// <summary>The Manager's Tenant id - the key used to address them for updates.</summary>
    public Guid TenantId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public Guid? ComplexId { get; set; }

    public string? ComplexName { get; set; }

    public LookupDto? City { get; set; }

    public LookupDto? District { get; set; }

    public LookupDto? Neighborhood { get; set; }

    public DateTime CreationTime { get; set; }
}
