using KatKat.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace KatKat.Permissions;

public class KatKatPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(KatKatPermissions.GroupName, L("Permission:KatKat"));

        var complexes = myGroup.AddPermission(KatKatPermissions.Complexes.Default, L("Permission:Complexes"));
        complexes.AddChild(KatKatPermissions.Complexes.Create, L("Permission:Complexes.Create"));
        complexes.AddChild(KatKatPermissions.Complexes.Update, L("Permission:Complexes.Update"));

        var buildings = myGroup.AddPermission(KatKatPermissions.Buildings.Default, L("Permission:Buildings"));
        buildings.AddChild(KatKatPermissions.Buildings.Create, L("Permission:Buildings.Create"));

        var flats = myGroup.AddPermission(KatKatPermissions.Flats.Default, L("Permission:Flats"));
        flats.AddChild(KatKatPermissions.Flats.Create, L("Permission:Flats.Create"));

        var flatMembers = myGroup.AddPermission(KatKatPermissions.FlatMembers.Default, L("Permission:FlatMembers"));
        flatMembers.AddChild(KatKatPermissions.FlatMembers.Approve, L("Permission:FlatMembers.Approve"));
        flatMembers.AddChild(KatKatPermissions.FlatMembers.PromoteToManager, L("Permission:FlatMembers.PromoteToManager"));

        var p2pRequests = myGroup.AddPermission(KatKatPermissions.P2PRequests.Default, L("Permission:P2PRequests"));
        p2pRequests.AddChild(KatKatPermissions.P2PRequests.Create, L("Permission:P2PRequests.Create"));

        var expenses = myGroup.AddPermission(KatKatPermissions.Expenses.Default, L("Permission:Expenses"));
        expenses.AddChild(KatKatPermissions.Expenses.Create, L("Permission:Expenses.Create"));

        var issues = myGroup.AddPermission(KatKatPermissions.Issues.Default, L("Permission:Issues"));
        issues.AddChild(KatKatPermissions.Issues.Create, L("Permission:Issues.Create"));
        issues.AddChild(KatKatPermissions.Issues.Resolve, L("Permission:Issues.Resolve"));

        var resources = myGroup.AddPermission(KatKatPermissions.Resources.Default, L("Permission:Resources"));
        resources.AddChild(KatKatPermissions.Resources.Create, L("Permission:Resources.Create"));

        var resourceReservations = myGroup.AddPermission(KatKatPermissions.ResourceReservations.Default, L("Permission:ResourceReservations"));
        resourceReservations.AddChild(KatKatPermissions.ResourceReservations.Create, L("Permission:ResourceReservations.Create"));

        var sosAlerts = myGroup.AddPermission(KatKatPermissions.SosAlerts.Default, L("Permission:SosAlerts"));
        sosAlerts.AddChild(KatKatPermissions.SosAlerts.Resolve, L("Permission:SosAlerts.Resolve"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<KatKatResource>(name);
    }
}
