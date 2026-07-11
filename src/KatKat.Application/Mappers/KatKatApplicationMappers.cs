using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;

namespace KatKat.Mappers;

[Mapper]
public partial class KatKatApplicationMappers
{
    [MapperIgnoreTarget(nameof(ComplexDto.City))]
    [MapperIgnoreTarget(nameof(ComplexDto.District))]
    [MapperIgnoreTarget(nameof(ComplexDto.Neighborhood))]
    public partial ComplexDto Map(Complex source);

    public partial BuildingDto Map(Building source);

    public partial FlatDto Map(Flat source);

    public partial FlatMemberDto Map(FlatMember source);

    public partial P2PRequestDto Map(P2PRequest source);

    public partial UserPreferenceDto Map(UserPreference source);

    public partial ExpenseDto Map(Expense source);

    public partial ExpenseShareDto Map(ExpenseShare source);

    public partial IssueDto Map(Issue source);

    public partial ResourceDto Map(Resource source);

    public partial ResourceReservationDto Map(ResourceReservation source);

    public partial SosAlertDto Map(SosAlert source);

    public partial CityDto Map(City source);

    [MapperIgnoreTarget(nameof(DistrictDto.City))]
    public partial DistrictDto Map(District source);

    [MapperIgnoreTarget(nameof(NeighborhoodDto.District))]
    public partial NeighborhoodDto Map(Neighborhood source);
}
