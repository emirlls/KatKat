using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class ResourceReservationMapper : MapperBase<ResourceReservation, ResourceReservationDto>
{
    public override partial ResourceReservationDto Map(ResourceReservation source);

    public override partial void Map(ResourceReservation source, ResourceReservationDto destination);
}
