using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>UserName is resolved separately (see FlatMemberAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class FlatMemberMapper : MapperBase<FlatMember, FlatMemberDto>
{
    [MapperIgnoreTarget(nameof(FlatMemberDto.UserName))]
    public override partial FlatMemberDto Map(FlatMember source);

    [MapperIgnoreTarget(nameof(FlatMemberDto.UserName))]
    public override partial void Map(FlatMember source, FlatMemberDto destination);
}
