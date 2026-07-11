namespace KatKat.Settings;

public static class KatKatSettings
{
    public const string GroupName = "KatKat";

    /// <summary>
    /// Radius (km) used by the nearby/map leaderboard - admin-configurable at runtime via ABP's
    /// Setting Management, seeded from <see cref="Constants.KatKatConsts.DefaultNearbyLeaderboardRadiusKm"/>.
    /// </summary>
    public const string NearbyLeaderboardRadiusKm = GroupName + ".NearbyLeaderboardRadiusKm";

    /// <summary>
    /// Per-HTTP-method distributed rate-limit knobs, admin-configurable at runtime via ABP's
    /// Setting Management (no redeploy needed), seeded from <see cref="Constants.RateLimitConsts"/>.
    /// Deliberately not visible to clients - exposing the exact thresholds would let an attacker
    /// calibrate abuse to stay just under the limit.
    /// </summary>
    public static class RateLimiting
    {
        public const string GroupName = KatKatSettings.GroupName + ".RateLimiting";

        public const string GetPermitLimit = GroupName + ".Get.PermitLimit";
        public const string GetWindowSeconds = GroupName + ".Get.WindowSeconds";
        public const string PostPermitLimit = GroupName + ".Post.PermitLimit";
        public const string PostWindowSeconds = GroupName + ".Post.WindowSeconds";
        public const string PutPermitLimit = GroupName + ".Put.PermitLimit";
        public const string PutWindowSeconds = GroupName + ".Put.WindowSeconds";
        public const string DeletePermitLimit = GroupName + ".Delete.PermitLimit";
        public const string DeleteWindowSeconds = GroupName + ".Delete.WindowSeconds";
        public const string PatchPermitLimit = GroupName + ".Patch.PermitLimit";
        public const string PatchWindowSeconds = GroupName + ".Patch.WindowSeconds";
    }
}
