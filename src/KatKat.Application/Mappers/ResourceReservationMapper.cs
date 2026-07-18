using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>FlatNumber/BuildingId/BuildingName are resolved separately (see ResourceReservationAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class ResourceReservationMapper : MapperBase<ResourceReservation, ResourceReservationDto>
{
    [MapperIgnoreTarget(nameof(ResourceReservationDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(ResourceReservationDto.BuildingId))]
    [MapperIgnoreTarget(nameof(ResourceReservationDto.BuildingName))]
    public override partial ResourceReservationDto Map(ResourceReservation source);

    [MapperIgnoreTarget(nameof(ResourceReservationDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(ResourceReservationDto.BuildingId))]
    [MapperIgnoreTarget(nameof(ResourceReservationDto.BuildingName))]
    public override partial void Map(ResourceReservation source, ResourceReservationDto destination);
}
