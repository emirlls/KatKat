using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class FlatMapper : MapperBase<Flat, FlatDto>
{
    public override partial FlatDto Map(Flat source);

    public override partial void Map(Flat source, FlatDto destination);
}
