using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>BuildingName is resolved separately (see IssueAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class IssueMapper : MapperBase<Issue, IssueDto>
{
    [MapperIgnoreTarget(nameof(IssueDto.BuildingName))]
    public override partial IssueDto Map(Issue source);

    [MapperIgnoreTarget(nameof(IssueDto.BuildingName))]
    public override partial void Map(Issue source, IssueDto destination);
}
