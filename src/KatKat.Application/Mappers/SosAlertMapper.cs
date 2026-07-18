using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>FlatNumber is resolved separately (see SosAlertAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class SosAlertMapper : MapperBase<SosAlert, SosAlertDto>
{
    [MapperIgnoreTarget(nameof(SosAlertDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(SosAlertDto.BuildingName))]
    public override partial SosAlertDto Map(SosAlert source);

    [MapperIgnoreTarget(nameof(SosAlertDto.FlatNumber))]
    [MapperIgnoreTarget(nameof(SosAlertDto.BuildingName))]
    public override partial void Map(SosAlert source, SosAlertDto destination);
}
