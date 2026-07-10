using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IExpenseRepository : IKatKatRepository<Expense, Guid>
{
    Task<List<Expense>> GetListByComplexAsync(Guid complexId);
}
