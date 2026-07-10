using System;
using System.Threading.Tasks;
using KatKat.Complexes;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Complexes;

public class EfCoreComplexRepository : EfCoreRepository<KatKatDbContext, Complex, Guid>, IComplexRepository
{
    public EfCoreComplexRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.Name == name && (excludedId == null || x.Id != excludedId));
    }
}
