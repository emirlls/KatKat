using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class FlatDto : FullAuditedEntityDto<Guid>
{
    public Guid BuildingId { get; set; }

    public string FlatNumber { get; set; } = null!;

    public int? FloorNumber { get; set; }

    public decimal ShareFactor { get; set; }
}
