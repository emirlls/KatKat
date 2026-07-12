using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Data;
using KatKat.Dtos;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace KatKat.Services;

public class AccountAppService : KatKatAppService, IAccountAppService
{
    private readonly IdentityUserManager _userManager;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly TenantManager _tenantManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly KatKatRoleDataSeedContributor _roleDataSeedContributor;
    private readonly IDataFilter _dataFilter;
    private readonly ILookupNormalizer _lookupNormalizer;

    public AccountAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        IGuidGenerator guidGenerator,
        TenantManager tenantManager,
        ITenantRepository tenantRepository,
        KatKatRoleDataSeedContributor roleDataSeedContributor,
        IDataFilter dataFilter,
        ILookupNormalizer lookupNormalizer)
    {
        _userManager = userManager;
        _identityUserRepository = identityUserRepository;
        _guidGenerator = guidGenerator;
        _tenantManager = tenantManager;
        _tenantRepository = tenantRepository;
        _roleDataSeedContributor = roleDataSeedContributor;
        _dataFilter = dataFilter;
        _lookupNormalizer = lookupNormalizer;
    }

    public async Task CreateManagerAsync(CreateManagerDto input)
    {
        var tenant = await _tenantManager.CreateAsync(input.UserName);
        await _tenantRepository.InsertAsync(tenant, autoSave: true);

        using (CurrentTenant.Change(tenant.Id))
        {
            // Manager/Resident roles - and the Manager role's permission grants - are themselves
            // tenant-scoped (IdentityRole is IMultiTenant), so every new tenant needs its own copy;
            // the host-level seeding that already ran once at startup never reaches this tenant.
            await _roleDataSeedContributor.SeedAsync(new DataSeedContext(tenant.Id));

            var user = new Volo.Abp.Identity.IdentityUser(_guidGenerator.Create(), input.UserName, input.Email, tenant.Id);
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();
            (await _userManager.AddToRoleAsync(user, KatKatRoleConsts.ManagerRoleName)).CheckErrors();
        }
    }

    public async Task<Guid?> GetTenantIdByUserNameAsync(string userName)
    {
        // Anonymous caller has no tenant context yet, so the lookup itself must cross every tenant.
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var normalizedUserName = _lookupNormalizer.NormalizeName(userName);
            var user = await _identityUserRepository.FindByNormalizedUserNameAsync(normalizedUserName);
            return user?.TenantId;
        }
    }
}
