using System;
using KatKat.Dtos.Common;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ComplexDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public LookupDto City { get; set; } = null!;

    public LookupDto District { get; set; } = null!;

    public LookupDto Neighborhood { get; set; } = null!;

    public string? Address { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime SubscriptionStartDate { get; set; }

    public DateTime? SubscriptionEndDate { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// Only populated for a single-Complex fetch (GetAsync/GetMyComplexAsync) - left at the
    /// default 0/null for bulk search results, which never display this summary info.
    /// </summary>
    public int BuildingCount { get; set; }

    public int FlatCount { get; set; }

    public string? ManagerUserName { get; set; }
}
