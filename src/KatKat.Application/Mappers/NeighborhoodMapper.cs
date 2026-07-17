using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>District is resolved separately and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class NeighborhoodMapper : MapperBase<Neighborhood, NeighborhoodDto>
{
    [MapperIgnoreTarget(nameof(NeighborhoodDto.District))]
    public override partial NeighborhoodDto Map(Neighborhood source);

    [MapperIgnoreTarget(nameof(NeighborhoodDto.District))]
    public override partial void Map(Neighborhood source, NeighborhoodDto destination);
}
