namespace KatKat.Constants;

/// <summary>
/// Self-registration roles (see AccountAppService.RegisterAsync). Distinct from ABP's own
/// "admin" role (full platform administration incl. Identity/Setting/Tenant management) -
/// Manager only ever gets KatKat's own permissions (KatKatPermissions.GetAll()), never ABP's.
/// </summary>
public static class KatKatRoleConsts
{
    public const string ManagerRoleName = "Manager";

    public const string ResidentRoleName = "Resident";
}
