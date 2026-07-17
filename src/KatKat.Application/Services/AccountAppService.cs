using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Data;
using KatKat.Dtos;
using KatKat.Dtos.Common;
using KatKat.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
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
    private readonly IComplexRepository _complexRepository;
    private readonly LocationLookupResolver _locationLookupResolver;

    public AccountAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        IGuidGenerator guidGenerator,
        TenantManager tenantManager,
        ITenantRepository tenantRepository,
        KatKatRoleDataSeedContributor roleDataSeedContributor,
        IDataFilter dataFilter,
        ILookupNormalizer lookupNormalizer,
        IComplexRepository complexRepository,
        LocationLookupResolver locationLookupResolver)
    {
        _userManager = userManager;
        _identityUserRepository = identityUserRepository;
        _guidGenerator = guidGenerator;
        _tenantManager = tenantManager;
        _tenantRepository = tenantRepository;
        _roleDataSeedContributor = roleDataSeedContributor;
        _dataFilter = dataFilter;
        _lookupNormalizer = lookupNormalizer;
        _complexRepository = complexRepository;
        _locationLookupResolver = locationLookupResolver;
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
            user.SetPhoneNumber(input.PhoneNumber, confirmed: false);
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

    public async Task<List<ManagerListItemDto>> GetManagersAsync(
        int? cityId = null, int? districtId = null, int? neighborhoodId = null, string? name = null,
        int maxResultCount = KatKatConsts.DefaultSearchMaxResultCount)
    {
        var tenants = await _tenantRepository.GetListAsync();

        var rows = new List<(Tenant Tenant, Volo.Abp.Identity.IdentityUser Manager, Entities.Complex? Complex)>();
        foreach (var tenant in tenants)
        {
            try
            {
                using (CurrentTenant.Change(tenant.Id))
                {
                    var manager = await FindTenantManagerAsync();
                    if (manager == null)
                    {
                        continue;
                    }

                    var complex = (await _complexRepository.GetListAsync()).FirstOrDefault();
                    rows.Add((tenant, manager, complex));
                }
            }
            catch (Exception ex)
            {
                // One tenant's bad data (or a transient failure) must not deny the admin visibility
                // into every OTHER tenant's managers - skip it and keep going.
                Logger.LogWarning(ex, "Skipping tenant {TenantId} in the admin managers directory", tenant.Id);
            }
        }

        var hierarchies = await _locationLookupResolver.ResolveNeighborhoodHierarchiesAsync(
            rows.Where(r => r.Complex != null).Select(r => r.Complex!.NeighborhoodId));

        var dtos = rows.Select(r =>
        {
            // TryGetValue, not the indexer: a Complex whose Neighborhood was since deleted must not
            // 500 the whole cross-tenant listing - it just shows without a resolved location.
            (LookupDto City, LookupDto District, LookupDto Neighborhood) hierarchy = default;
            var hasHierarchy = r.Complex != null && hierarchies.TryGetValue(r.Complex.NeighborhoodId, out hierarchy);
            return new ManagerListItemDto
            {
                Id = r.Manager.Id,
                TenantId = r.Tenant.Id,
                UserName = r.Manager.UserName,
                Email = r.Manager.Email,
                PhoneNumber = r.Manager.PhoneNumber,
                IsActive = r.Manager.IsActive,
                ComplexId = r.Complex?.Id,
                ComplexName = r.Complex?.Name,
                City = hasHierarchy ? hierarchy.City : null,
                District = hasHierarchy ? hierarchy.District : null,
                Neighborhood = hasHierarchy ? hierarchy.Neighborhood : null,
                CreationTime = r.Manager.CreationTime,
            };
        });

        if (neighborhoodId != null)
        {
            dtos = dtos.Where(d => d.Neighborhood?.Id == neighborhoodId);
        }
        else if (districtId != null)
        {
            dtos = dtos.Where(d => d.District?.Id == districtId);
        }
        else if (cityId != null)
        {
            dtos = dtos.Where(d => d.City?.Id == cityId);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.ToLowerInvariant();
            dtos = dtos.Where(d =>
                d.UserName.ToLowerInvariant().Contains(normalizedName) ||
                d.Email.ToLowerInvariant().Contains(normalizedName));
        }

        return dtos.OrderBy(d => d.UserName).Take(maxResultCount).ToList();
    }

    public async Task<ManagerListItemDto> UpdateManagerAsync(Guid tenantId, UpdateManagerDto input)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var manager = await FindTenantManagerAsync()
                ?? throw new EntityNotFoundException(typeof(Volo.Abp.Identity.IdentityUser));

            (await _userManager.SetUserNameAsync(manager, input.UserName)).CheckErrors();
            (await _userManager.SetEmailAsync(manager, input.Email)).CheckErrors();
            (await _userManager.SetPhoneNumberAsync(manager, input.PhoneNumber)).CheckErrors();

            return await BuildManagerDtoAsync(tenantId, manager);
        }
    }

    /// <summary>
    /// Admin-only: reversibly blocks (or restores) the Manager's ability to log in, without
    /// touching their Tenant or any data - the safe alternative to <see cref="DeleteManagerAsync"/>.
    /// </summary>
    public async Task<ManagerListItemDto> SetManagerActiveAsync(Guid tenantId, bool isActive)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var manager = await FindTenantManagerAsync()
                ?? throw new EntityNotFoundException(typeof(Volo.Abp.Identity.IdentityUser));

            manager.SetIsActive(isActive);
            (await _userManager.UpdateAsync(manager)).CheckErrors();

            return await BuildManagerDtoAsync(tenantId, manager);
        }
    }

    /// <summary>
    /// Admin-only: permanently removes a Manager and their entire site - every IdentityUser in the
    /// Tenant (the Manager and any Residents) plus the Tenant itself. Everything is soft-deleted
    /// (this codebase's usual FullAuditedAggregateRoot convention), so it's recoverable at the
    /// database level if ever truly needed, but immediately and irreversibly inaccessible through
    /// the app - the Tenant no longer resolves, so no login into it is possible again. Tenant-scoped
    /// business data (Complexes/Buildings/Flats/...) is intentionally left in place rather than
    /// cascade-purged: once the Tenant is gone, nothing can ever reach it again anyway.
    /// </summary>
    public async Task DeleteManagerAsync(Guid tenantId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            // includeDetails: true is required here - IdentityUserManager.DeleteAsync() clears the
            // user's Claims/Roles/Tokens/Logins/OrganizationUnits collections unconditionally, which
            // throws a NullReferenceException if they weren't eager-loaded first.
            var users = await _identityUserRepository.GetListAsync(includeDetails: true);
            foreach (var user in users)
            {
                (await _userManager.DeleteAsync(user)).CheckErrors();
            }
        }

        await _tenantRepository.DeleteAsync(tenantId, autoSave: true);
    }

    /// <summary>Finds the (exactly one, by construction) Manager-role user in the CURRENT ambient tenant.</summary>
    private async Task<Volo.Abp.Identity.IdentityUser?> FindTenantManagerAsync()
    {
        var users = await _identityUserRepository.GetListAsync();
        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, KatKatRoleConsts.ManagerRoleName))
            {
                return user;
            }
        }

        return null;
    }

    /// <summary>Must be called from within the target tenant's <see cref="ICurrentTenant"/> scope.</summary>
    private async Task<ManagerListItemDto> BuildManagerDtoAsync(Guid tenantId, Volo.Abp.Identity.IdentityUser manager)
    {
        var complex = (await _complexRepository.GetListAsync()).FirstOrDefault();
        var hasHierarchy = false;
        (LookupDto City, LookupDto District, LookupDto Neighborhood) hierarchy = default;
        if (complex != null)
        {
            var hierarchies = await _locationLookupResolver.ResolveNeighborhoodHierarchiesAsync(new[] { complex.NeighborhoodId });
            hasHierarchy = hierarchies.TryGetValue(complex.NeighborhoodId, out hierarchy);
        }

        return new ManagerListItemDto
        {
            Id = manager.Id,
            TenantId = tenantId,
            UserName = manager.UserName,
            Email = manager.Email,
            PhoneNumber = manager.PhoneNumber,
            IsActive = manager.IsActive,
            ComplexId = complex?.Id,
            ComplexName = complex?.Name,
            City = hasHierarchy ? hierarchy.City : null,
            District = hasHierarchy ? hierarchy.District : null,
            Neighborhood = hasHierarchy ? hierarchy.Neighborhood : null,
            CreationTime = manager.CreationTime,
        };
    }
}
