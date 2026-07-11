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
}
