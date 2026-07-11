using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class FlatMemberMapper : MapperBase<FlatMember, FlatMemberDto>
{
    public override partial FlatMemberDto Map(FlatMember source);

    public override partial void Map(FlatMember source, FlatMemberDto destination);
}
