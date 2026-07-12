using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Permissions;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace KatKat.Data;

/// <summary>
/// Seeds the KatKat "Manager"/"Resident" roles. Runs once at host startup (host-level, no
/// tenant) AND once per new Tenant right after AccountAppService.CreateManagerAsync creates one -
/// IdentityRole is itself tenant-scoped, so a role seeded for one tenant is invisible to another;
/// every tenant needs its own copy of these two roles. Idempotent per (tenant, role name) pair, so
/// it's a safe no-op on every subsequent call for the same tenant.
/// </summary>
public class KatKatRoleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string RolePermissionProviderName = "R";

    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionManager _permissionManager;
    private readonly IGuidGenerator _guidGenerator;

    public KatKatRoleDataSeedContributor(
        IdentityRoleManager roleManager, IPermissionManager permissionManager, IGuidGenerator guidGenerator)
    {
        _roleManager = roleManager;
        _permissionManager = permissionManager;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await EnsureRoleExistsAsync(KatKatRoleConsts.ResidentRoleName, context.TenantId))
        {
            // Residents only get the subset of KatKat permissions that are genuinely resident-facing
            // (booking an existing Resource) - everything else (Complex/Building/Flat management,
            // raising Expenses, resolving Issues/SosAlerts, location management) stays Manager-only.
            await _permissionManager.SetAsync(
                KatKatPermissions.ResourceReservations.Create, RolePermissionProviderName, KatKatRoleConsts.ResidentRoleName, isGranted: true);
        }

        if (await EnsureRoleExistsAsync(KatKatRoleConsts.ManagerRoleName, context.TenantId))
        {
            foreach (var permissionName in KatKatPermissions.GetAll())
            {
                await _permissionManager.SetAsync(permissionName, RolePermissionProviderName, KatKatRoleConsts.ManagerRoleName, isGranted: true);
            }
        }
    }

    /// <summary>Returns true only when the role was just created (so callers can seed its permissions once).</summary>
    private async Task<bool> EnsureRoleExistsAsync(string roleName, Guid? tenantId)
    {
        if (await _roleManager.FindByNameAsync(roleName) != null)
        {
            return false;
        }

        (await _roleManager.CreateAsync(new Volo.Abp.Identity.IdentityRole(_guidGenerator.Create(), roleName, tenantId))).CheckErrors();
        return true;
    }
}
