using System;
using System.Collections.Generic;
using KatKat.Constants;

namespace KatKat.Dtos;

/// <summary>Admin-only cross-tenant read view of a Complex's full structure, for the "Tüm Siteler" drill-down.</summary>
public class AdminSiteDetailDto
{
    public ComplexDto Complex { get; set; } = null!;

    public Guid? TenantId { get; set; }

    public List<AdminBuildingDetailDto> Buildings { get; set; } = new();
}

public class AdminBuildingDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int? FloorCount { get; set; }

    public List<AdminFlatDetailDto> Flats { get; set; } = new();
}

public class AdminFlatDetailDto
{
    public Guid Id { get; set; }

    public string FlatNumber { get; set; } = null!;

    public int? FloorNumber { get; set; }

    public decimal ShareFactor { get; set; }

    public List<AdminResidentDto> Residents { get; set; } = new();
}

/// <summary>A Flat's member, resolved down to a display-safe username - never a raw user id.</summary>
public class AdminResidentDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public FlatMemberRole Role { get; set; }
}
