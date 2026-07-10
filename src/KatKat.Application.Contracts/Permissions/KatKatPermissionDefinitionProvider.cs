using KatKat.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace KatKat.Permissions;

public class KatKatPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(KatKatPermissions.GroupName, L("Permission:KatKat"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<KatKatResource>(name);
    }
}
