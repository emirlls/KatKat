using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IExpenseAppService : IApplicationService
{
    Task<ExpenseDto> GetAsync(Guid id);

    Task<List<ExpenseDto>> GetListByComplexAsync(Guid complexId);

    /// <summary>Creates the Expense and calculates every flat's share in one step.</summary>
    Task<ExpenseDto> CreateAsync(CreateExpenseDto input);

    Task<List<ExpenseShareDto>> GetSharesByExpenseAsync(Guid expenseId);

    /// <summary>The calling resident's own dues across their Flat.</summary>
    Task<List<ExpenseShareDto>> GetSharesByFlatAsync(Guid flatId);

    /// <summary>Marks a share as paid - only a member of the owning Flat may call this.</summary>
    Task<ExpenseShareDto> PayShareAsync(Guid shareId);
}
