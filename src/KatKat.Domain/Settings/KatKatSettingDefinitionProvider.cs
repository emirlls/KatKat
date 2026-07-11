using System.Globalization;
using KatKat.Constants;
using KatKat.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace KatKat.Settings;

public class KatKatSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                KatKatSettings.NearbyLeaderboardRadiusKm,
                KatKatConsts.DefaultNearbyLeaderboardRadiusKm.ToString(CultureInfo.InvariantCulture),
                L("Setting:NearbyLeaderboardRadiusKm"),
                isVisibleToClients: true));

        // Rate-limiting knobs - deliberately not visible to clients, since exposing the exact
        // thresholds would let an attacker calibrate abuse to stay just under the limit.
        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.GetPermitLimit,
                RateLimitConsts.DefaultGetPermitLimit.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Get.PermitLimit"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.GetWindowSeconds,
                RateLimitConsts.DefaultGetWindowSeconds.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Get.WindowSeconds"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PostPermitLimit,
                RateLimitConsts.DefaultPostPermitLimit.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Post.PermitLimit"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PostWindowSeconds,
                RateLimitConsts.DefaultPostWindowSeconds.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Post.WindowSeconds"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PutPermitLimit,
                RateLimitConsts.DefaultPutPermitLimit.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Put.PermitLimit"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PutWindowSeconds,
                RateLimitConsts.DefaultPutWindowSeconds.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Put.WindowSeconds"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.DeletePermitLimit,
                RateLimitConsts.DefaultDeletePermitLimit.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Delete.PermitLimit"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.DeleteWindowSeconds,
                RateLimitConsts.DefaultDeleteWindowSeconds.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Delete.WindowSeconds"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PatchPermitLimit,
                RateLimitConsts.DefaultPatchPermitLimit.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Patch.PermitLimit"),
                isVisibleToClients: false));

        context.Add(
            new SettingDefinition(
                KatKatSettings.RateLimiting.PatchWindowSeconds,
                RateLimitConsts.DefaultPatchWindowSeconds.ToString(CultureInfo.InvariantCulture),
                L("Setting:RateLimiting.Patch.WindowSeconds"),
                isVisibleToClients: false));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<KatKatResource>(name);
    }
}
