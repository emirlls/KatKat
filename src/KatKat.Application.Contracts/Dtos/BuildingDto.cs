using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class BuildingDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public string Name { get; set; } = null!;

    public int? FloorCount { get; set; }
}
