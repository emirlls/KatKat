using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IExpenseShareRepository : IKatKatRepository<ExpenseShare, Guid>
{
    Task<List<ExpenseShare>> GetListByExpenseAsync(Guid expenseId);

    Task<List<ExpenseShare>> GetListByFlatAsync(Guid flatId);

    /// <summary>
    /// Average number of days between an Expense's IssuedAt and its ExpenseShares' PaidAt,
    /// across shares paid since <paramref name="since"/>. Null if nothing has been paid yet.
    /// </summary>
    Task<decimal?> GetAveragePaymentDelayDaysAsync(Guid complexId, DateTime since);
}
