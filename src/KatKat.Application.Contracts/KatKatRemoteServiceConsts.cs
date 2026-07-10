namespace KatKat;

public class KatKatRemoteServiceConsts
{
    public const string RemoteServiceName = "KatKat";

    public const string DisplayName = "KatKat API";

    public const string ModuleName = "katKat";

    /// <summary>
    /// Prefix every KatKat controller route starts with - used to tell this module's own
    /// endpoints apart from ABP's built-in module APIs (Identity, Tenant/Permission/Setting
    /// Management, Account...) when filtering the Swagger document.
    /// </summary>
    public const string RoutePathPrefix = "api/katkat";
}
