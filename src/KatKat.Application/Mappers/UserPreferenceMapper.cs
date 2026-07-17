using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class UserPreferenceMapper : MapperBase<UserPreference, UserPreferenceDto>
{
    public override partial UserPreferenceDto Map(UserPreference source);

    public override partial void Map(UserPreference source, UserPreferenceDto destination);
}
