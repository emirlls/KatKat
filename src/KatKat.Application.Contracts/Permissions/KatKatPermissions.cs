using Volo.Abp.Reflection;

namespace KatKat.Permissions;

public class KatKatPermissions
{
    public const string GroupName = "KatKat";

    public static class Complexes
    {
        public const string Default = GroupName + ".Complexes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Buildings
    {
        public const string Default = GroupName + ".Buildings";
        public const string Create = Default + ".Create";
    }

    public static class Flats
    {
        public const string Default = GroupName + ".Flats";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class FlatMembers
    {
        public const string Default = GroupName + ".FlatMembers";
        public const string Approve = Default + ".Approve";
        public const string PromoteToManager = Default + ".PromoteToManager";
    }

    public static class P2PRequests
    {
        public const string Default = GroupName + ".P2PRequests";
        public const string Create = Default + ".Create";
    }

    public static class Expenses
    {
        public const string Default = GroupName + ".Expenses";
        public const string Create = Default + ".Create";
    }

    public static class Issues
    {
        public const string Default = GroupName + ".Issues";
        public const string Create = Default + ".Create";
        public const string Resolve = Default + ".Resolve";
    }

    public static class Resources
    {
        public const string Default = GroupName + ".Resources";
        public const string Create = Default + ".Create";
    }

    public static class ResourceReservations
    {
        public const string Default = GroupName + ".ResourceReservations";
        public const string Create = Default + ".Create";
    }

    public static class SosAlerts
    {
        public const string Default = GroupName + ".SosAlerts";
        public const string Resolve = Default + ".Resolve";
    }

    public static class Cities
    {
        public const string Default = GroupName + ".Cities";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Districts
    {
        public const string Default = GroupName + ".Districts";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Neighborhoods
    {
        public const string Default = GroupName + ".Neighborhoods";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(KatKatPermissions));
    }
}
