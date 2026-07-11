using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class CityMapper : MapperBase<City, CityDto>
{
    public override partial CityDto Map(City source);

    public override partial void Map(City source, CityDto destination);
}
