using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Permissions;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace KatKat.Data;

/// <summary>
/// Seeds (and reconciles) the KatKat "Manager"/"Resident" roles. Runs at host startup, once per new
/// Tenant right after AccountAppService.CreateManagerAsync creates one, and again for every existing
/// tenant on startup (see KatKatHttpApiHostModule.ReconcileTenantRolesAsync). IdentityRole is itself
/// tenant-scoped, so each tenant needs its own copy of these two roles.
///
/// Fully idempotent: it (re)applies the CURRENT permission set on every run - not only when the role
/// is first created - so a permission added in a later build reaches tenants that predate it without
/// any data loss.
/// </summary>
public class KatKatRoleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string RolePermissionProviderName = "R";

    /// <summary>
    /// Resident-facing capabilities: booking a Resource, reporting an Issue, raising a neighbor
    /// request, and acknowledging a neighbor's SOS ("Yardım Ulaştı"). Everything else -
    /// Complex/Building/Flat management, splitting Expenses, resolving Issues, approving
    /// reservations, location management - stays Manager-only.
    /// </summary>
    private static readonly string[] ResidentPermissions =
    {
        KatKatPermissions.ResourceReservations.Create,
        KatKatPermissions.Issues.Create,
        KatKatPermissions.P2PRequests.Create,
        KatKatPermissions.SosAlerts.Resolve,
    };

    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionManager _permissionManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public KatKatRoleDataSeedContributor(
        IdentityRoleManager roleManager,
        IPermissionManager permissionManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _roleManager = roleManager;
        _permissionManager = permissionManager;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Pin the ambient tenant so role lookups/creates and permission grants all land in the
        // tenant we're seeding (harmless no-op when the caller already changed it).
        using (_currentTenant.Change(context.TenantId))
        {
            await EnsureRoleWithGrantsAsync(KatKatRoleConsts.ResidentRoleName, context.TenantId, ResidentPermissions);
            await EnsureRoleWithGrantsAsync(KatKatRoleConsts.ManagerRoleName, context.TenantId, KatKatPermissions.GetAll());
        }
    }

    private async Task EnsureRoleWithGrantsAsync(string roleName, Guid? tenantId, IReadOnlyCollection<string> permissions)
    {
        if (await _roleManager.FindByNameAsync(roleName) == null)
        {
            (await _roleManager.CreateAsync(
                new Volo.Abp.Identity.IdentityRole(_guidGenerator.Create(), roleName, tenantId))).CheckErrors();
        }

        // Reconcile every run: SetAsync is an idempotent upsert, so re-granting the current set is
        // cheap and lets existing tenants pick up permissions introduced after their role was created.
        foreach (var permissionName in permissions)
        {
            await _permissionManager.SetAsync(permissionName, RolePermissionProviderName, roleName, isGranted: true);
        }
    }
}
