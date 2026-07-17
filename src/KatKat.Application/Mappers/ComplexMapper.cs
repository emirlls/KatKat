using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>City/District/Neighborhood are resolved separately (see LocationLookupResolver) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class ComplexMapper : MapperBase<Complex, ComplexDto>
{
    [MapperIgnoreTarget(nameof(ComplexDto.City))]
    [MapperIgnoreTarget(nameof(ComplexDto.District))]
    [MapperIgnoreTarget(nameof(ComplexDto.Neighborhood))]
    public override partial ComplexDto Map(Complex source);

    [MapperIgnoreTarget(nameof(ComplexDto.City))]
    [MapperIgnoreTarget(nameof(ComplexDto.District))]
    [MapperIgnoreTarget(nameof(ComplexDto.Neighborhood))]
    public override partial void Map(Complex source, ComplexDto destination);
}
