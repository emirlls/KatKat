using KatKat.Dtos.Common;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class DistrictDto : FullAuditedEntityDto<int>
{
    public LookupDto City { get; set; } = null!;

    public string Name { get; set; } = null!;
}
