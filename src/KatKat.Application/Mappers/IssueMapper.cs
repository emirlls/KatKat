using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class IssueMapper : MapperBase<Issue, IssueDto>
{
    public override partial IssueDto Map(Issue source);

    public override partial void Map(Issue source, IssueDto destination);
}
