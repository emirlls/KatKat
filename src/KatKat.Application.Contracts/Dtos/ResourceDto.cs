using System;
using KatKat.Resources;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ResourceDto : FullAuditedEntityDto<Guid>
{
    public Guid ComplexId { get; set; }

    public string Name { get; set; } = null!;

    public ResourceType Type { get; set; }
}
