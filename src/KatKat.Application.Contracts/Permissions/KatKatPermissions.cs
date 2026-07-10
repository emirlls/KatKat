using Volo.Abp.Reflection;

namespace KatKat.Permissions;

public class KatKatPermissions
{
    public const string GroupName = "KatKat";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(KatKatPermissions));
    }
}
