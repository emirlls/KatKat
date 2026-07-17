using KatKat.Dtos;
using KatKat.Entities;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace KatKat.Mappers;

[Mapper]
public partial class ExpenseMapper : MapperBase<Expense, ExpenseDto>
{
    public override partial ExpenseDto Map(Expense source);

    public override partial void Map(Expense source, ExpenseDto destination);
}
