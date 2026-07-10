using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreExpenseShareRepository : KatKatEfCoreRepository<ExpenseShare, Guid>, IExpenseShareRepository
{
    public EfCoreExpenseShareRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ExpenseShare>> GetListByExpenseAsync(Guid expenseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.ExpenseId == expenseId).ToListAsync();
    }

    public async Task<List<ExpenseShare>> GetListByFlatAsync(Guid flatId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.FlatId == flatId).ToListAsync();
    }

    public async Task<decimal?> GetAveragePaymentDelayDaysAsync(Guid complexId, DateTime since)
    {
        var dbContext = await GetDbContextAsync();

        var paidPairs = await (
            from share in dbContext.ExpenseShares
            join expense in dbContext.Expenses on share.ExpenseId equals expense.Id
            where expense.ComplexId == complexId &&
                  share.IsPaid &&
                  share.PaidAt != null &&
                  share.PaidAt >= since
            select new { expense.IssuedAt, share.PaidAt }
        ).ToListAsync();

        if (paidPairs.Count == 0)
        {
            return null;
        }

        return (decimal)paidPairs.Average(pair => (pair.PaidAt!.Value - pair.IssuedAt).TotalDays);
    }
}
