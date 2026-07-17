using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace KatKat.OpenIddict;

/// <summary>
/// Seeds the OpenIddict scope/applications this Host issues its own tokens for, since it is a
/// self-contained AuthServer + resource server (no separate AuthServer host in this solution).
/// Driven entirely by the "OpenIddict:Applications" section of appsettings.json - add a client
/// there (ClientId/RootUrl) and it is created/updated here idempotently on every app start.
/// </summary>
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = KatKatRemoteServiceConsts.RemoteServiceName,
            DisplayName = KatKatRemoteServiceConsts.DisplayName,
            Resources = { KatKatRemoteServiceConsts.RemoteServiceName }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string>
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            KatKatRemoteServiceConsts.RemoteServiceName
        };

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");

        // The KatKat-UI SPA - Resource Owner Password Credentials flow, no client secret (public client).
        var appClientId = configurationSection["KatKat_App:ClientId"];
        if (!string.IsNullOrWhiteSpace(appClientId))
        {
            var appRootUrl = configurationSection["KatKat_App:RootUrl"]?.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: appClientId,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "KatKat-UI",
                secret: null,
                grantTypes: new List<string>
                {
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.RefreshToken,
                    OpenIddictConstants.GrantTypes.ClientCredentials
                },
                scopes: commonScopes,
                redirectUris: appRootUrl == null ? null : new List<string> { appRootUrl },
                postLogoutRedirectUris: appRootUrl == null ? null : new List<string> { appRootUrl },
                clientUri: appRootUrl
            );
        }

        // Swagger UI's own OAuth "Authorize" button.
        var swaggerClientId = configurationSection["KatKat_Swagger:ClientId"];
        if (!string.IsNullOrWhiteSpace(swaggerClientId))
        {
            var swaggerRootUrl = configurationSection["KatKat_Swagger:RootUrl"]?.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: swaggerClientId,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger UI",
                secret: null,
                grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode },
                scopes: commonScopes,
                redirectUris: new List<string> { $"{swaggerRootUrl}/swagger/oauth2-redirect.html" },
                clientUri: swaggerRootUrl
            );
        }
    }
}
