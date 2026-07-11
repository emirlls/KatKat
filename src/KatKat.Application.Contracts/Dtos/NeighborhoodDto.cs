using KatKat.Dtos.Common;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class NeighborhoodDto : FullAuditedEntityDto<int>
{
    public LookupDto District { get; set; } = null!;

    public string Name { get; set; } = null!;
}
