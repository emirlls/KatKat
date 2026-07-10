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

public class EfCoreExpenseRepository : KatKatEfCoreRepository<Expense, Guid>, IExpenseRepository
{
    public EfCoreExpenseRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Expense>> GetListByComplexAsync(Guid complexId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ComplexId == complexId)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync();
    }
}
