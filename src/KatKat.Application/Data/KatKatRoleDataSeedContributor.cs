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
/// Seeds the self-registration roles (see AccountAppService.RegisterAsync). Idempotent - a
/// role that already exists is left untouched, so this is a safe no-op on every subsequent
/// application start. Manager only ever gets KatKat's own permissions, never ABP's own
/// Identity/Setting/Tenant management ones (those stay exclusive to the seeded "admin" role).
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
        await EnsureRoleExistsAsync(KatKatRoleConsts.ResidentRoleName);

        if (await EnsureRoleExistsAsync(KatKatRoleConsts.ManagerRoleName))
        {
            foreach (var permissionName in KatKatPermissions.GetAll())
            {
                await _permissionManager.SetAsync(permissionName, RolePermissionProviderName, KatKatRoleConsts.ManagerRoleName, isGranted: true);
            }
        }
    }

    /// <summary>Returns true only when the role was just created (so callers can seed its permissions once).</summary>
    private async Task<bool> EnsureRoleExistsAsync(string roleName)
    {
        if (await _roleManager.FindByNameAsync(roleName) != null)
        {
            return false;
        }

        (await _roleManager.CreateAsync(new Volo.Abp.Identity.IdentityRole(_guidGenerator.Create(), roleName))).CheckErrors();
        return true;
    }
}
