using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

/// <summary>FlatNumber is resolved separately (see ExpenseAppService) and denormalized onto the Dto after mapping.</summary>
[Mapper]
public partial class ExpenseShareMapper : MapperBase<ExpenseShare, ExpenseShareDto>
{
    [MapperIgnoreTarget(nameof(ExpenseShareDto.FlatNumber))]
    public override partial ExpenseShareDto Map(ExpenseShare source);

    [MapperIgnoreTarget(nameof(ExpenseShareDto.FlatNumber))]
    public override partial void Map(ExpenseShare source, ExpenseShareDto destination);
}
