using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class ResourceMapper : MapperBase<Resource, ResourceDto>
{
    public override partial ResourceDto Map(Resource source);

    public override partial void Map(Resource source, ResourceDto destination);
}
