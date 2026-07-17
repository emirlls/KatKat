using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class ResidentInvitationMapper : MapperBase<ResidentInvitation, ResidentInvitationDto>
{
    public override partial ResidentInvitationDto Map(ResidentInvitation source);

    public override partial void Map(ResidentInvitation source, ResidentInvitationDto destination);
}
