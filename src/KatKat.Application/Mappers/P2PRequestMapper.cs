using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class P2PRequestMapper : MapperBase<P2PRequest, P2PRequestDto>
{
    public override partial P2PRequestDto Map(P2PRequest source);

    public override partial void Map(P2PRequest source, P2PRequestDto destination);
}
