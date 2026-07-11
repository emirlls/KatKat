using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class BuildingMapper : MapperBase<Building, BuildingDto>
{
    public override partial BuildingDto Map(Building source);

    public override partial void Map(Building source, BuildingDto destination);
}
