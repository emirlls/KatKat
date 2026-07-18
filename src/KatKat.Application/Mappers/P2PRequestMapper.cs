using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>FlatNumber/BuildingId/BuildingName are resolved separately (see P2PRequestAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class P2PRequestMapper : MapperBase<P2PRequest, P2PRequestDto>
{
    [MapperIgnoreTarget(nameof(P2PRequestDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(P2PRequestDto.BuildingId))]
    [MapperIgnoreTarget(nameof(P2PRequestDto.BuildingName))]
    public override partial P2PRequestDto Map(P2PRequest source);

    [MapperIgnoreTarget(nameof(P2PRequestDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(P2PRequestDto.BuildingId))]
    [MapperIgnoreTarget(nameof(P2PRequestDto.BuildingName))]
    public override partial void Map(P2PRequest source, P2PRequestDto destination);
}
