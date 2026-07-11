using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class CityDto : FullAuditedEntityDto<int>
{
    public string Name { get; set; } = null!;
}
