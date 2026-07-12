using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreResidentInvitationRepository : KatKatEfCoreRepository<ResidentInvitation, System.Guid>, IResidentInvitationRepository
{
    private readonly IDataFilter _dataFilter;

    public EfCoreResidentInvitationRepository(IDbContextProvider<KatKatDbContext> dbContextProvider, IDataFilter dataFilter)
        : base(dbContextProvider)
    {
        _dataFilter = dataFilter;
    }

    /// <summary>
    /// A resident redeeming a code doesn't know (and shouldn't have to send) which tenant it
    /// belongs to - that's exactly what this lookup determines - so the multi-tenancy filter is
    /// deliberately disabled here; every other query on this repository stays tenant-scoped.
    /// </summary>
    public async Task<ResidentInvitation?> FindByCodeAsync(string code)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
