using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>City is resolved separately and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class DistrictMapper : MapperBase<District, DistrictDto>
{
    [MapperIgnoreTarget(nameof(DistrictDto.City))]
    public override partial DistrictDto Map(District source);

    [MapperIgnoreTarget(nameof(DistrictDto.City))]
    public override partial void Map(District source, DistrictDto destination);
}
