using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OpenIddict.Server;
using KatKat.EntityFrameworkCore;
using KatKat.Hubs;
using KatKat.MultiTenancy;
using KatKat.RateLimiting;
using KatKat.ResponseWrapping;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.VirtualFileSystem;

namespace KatKat;

[DependsOn(
    typeof(KatKatApplicationModule),
    typeof(KatKatEntityFrameworkCoreModule),
    typeof(KatKatHttpApiModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpOpenIddictAspNetCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class KatKatHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // The Docker Compose setup deliberately serves plain HTTP (see docker-compose.yml) to
        // avoid dealing with dev-certificate trust inside containers; OpenIddict otherwise
        // rejects every token request over HTTP. The regular local-dev flow (dotnet run, HTTPS
        // via Kestrel dev-certs) keeps the default transport-security requirement.
        if (context.Services.GetHostingEnvironment().IsEnvironment("Docker"))
        {
            PreConfigure<OpenIddictServerBuilder>(builder =>
            {
                builder.UseAspNetCore().DisableTransportSecurityRequirement();
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        // AuditLogging's own tables (AbpAuditLogs, AbpEntityChanges...) are not wanted - KatKat's
        // entities already carry full audit fields (CreationTime/CreatorId/...) via
        // FullAuditedAggregateRoot, making a separate audit trail redundant for this module.
        Configure<AbpAuditingOptions>(options =>
        {
            options.IsEnabled = false;
        });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<KatKatDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}KatKat.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<KatKatDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}KatKat.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<KatKatApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}KatKat.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<KatKatApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}KatKat.Application", Path.DirectorySeparatorChar)));
            });
        }

        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                {KatKatRemoteServiceConsts.RemoteServiceName, KatKatRemoteServiceConsts.DisplayName}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = KatKatRemoteServiceConsts.DisplayName, Version = "v1"});

                // Only expose this module's own endpoints - ABP's built-in module APIs
                // (Identity, Tenant/Permission/Setting Management, Account...) stay out of Swagger.
                options.DocInclusionPredicate((docName, description) =>
                    description.RelativePath != null &&
                    description.RelativePath.StartsWith(
                        KatKatRemoteServiceConsts.RoutePathPrefix, StringComparison.OrdinalIgnoreCase));

                options.CustomSchemaIds(type => type.FullName);

                var httpApiXmlPath = Path.Combine(AppContext.BaseDirectory, "KatKat.HttpApi.xml");
                if (File.Exists(httpApiXmlPath))
                {
                    options.IncludeXmlComments(httpApiXmlPath);
                }

                var applicationContractsXmlPath = Path.Combine(AppContext.BaseDirectory, "KatKat.Application.Contracts.xml");
                if (File.Exists(applicationContractsXmlPath))
                {
                    options.IncludeXmlComments(applicationContractsXmlPath);
                }
            });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
        });

        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddAbpJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.Audience = KatKatRemoteServiceConsts.RemoteServiceName;

                // Without this, the JWT bearer handler remaps short claim names ("sub", "role")
                // to their long legacy URIs (ClaimTypes.NameIdentifier/Role) by default, which no
                // longer match AbpClaimTypes.UserId/Role ("sub"/"role") - breaking CurrentUser and
                // every permission check for every authenticated request.
                options.MapInboundClaims = false;

                // ASP.NET Core's role-based checks (ClaimsPrincipal.IsInRole, [Authorize(Roles = ...)])
                // read whichever claim type ClaimsIdentity.RoleClaimType points at - which defaults to
                // the long ClaimTypes.Role URI. Since MapInboundClaims=false keeps the short "role"
                // claim instead, that default never matches; point it at the short form so
                // [Authorize(Roles = "admin")] (used to gate Manager provisioning) actually works.
                options.TokenValidationParameters.RoleClaimType = AbpClaimTypes.Role;
            });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "KatKat:";
        });

        // AbpCachingStackExchangeRedisModule keeps its own internal multiplexer private (not
        // exposed via DI), so the dynamic rate limiter needs its own registration to talk to
        // Redis directly. Reuses the same "Redis:Configuration" connection string already used
        // below for data-protection key storage.
        context.Services.TryAddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!));
        context.Services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName(KatKatRemoteServiceConsts.RemoteServiceName);
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "KatKat-Protection-Keys");
        }

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        // Registered early (before routing/auth) so it wraps EVERY response for KatKat's own API
        // routes - including 401/403/429 short-circuits - in the { message, success, status, data }
        // envelope. Requests outside the API route prefix (Swagger, the SignalR hub, static
        // assets) are left untouched by the middleware's own path check.
        app.UseKatKatApiResponseWrapper();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes(KatKatRemoteServiceConsts.RemoteServiceName);
        });
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHub<KatKatHub>(KatKatHubConsts.RoutePath);
        });
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await base.OnApplicationInitializationAsync(context);

        using var scope = context.ServiceProvider.CreateScope();

        // Applies pending migrations on every startup so a fresh `docker compose up` (or a
        // fresh local Postgres) ends up with an up-to-date schema without a separate manual
        // `dotnet ef database update` step. Idempotent - a no-op once the schema is current.
        await MigrateDatabaseAsync(scope.ServiceProvider.GetRequiredService<IConfiguration>());

        // One-time bootstrap: seeds Turkey's 81 provinces into the City lookup table.
        // CityDataSeedContributor is idempotent (no-op once the table is non-empty), so this is
        // safe to run on every application start.
        await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
    }

    private static async Task MigrateDatabaseAsync(IConfiguration configuration)
    {
        var options = new DbContextOptionsBuilder<KatKatHttpApiHostMigrationsDbContext>()
            .UseNpgsql(configuration.GetConnectionString(KatKatDbProperties.ConnectionStringName))
            .Options;

        await using var migrationsDbContext = new KatKatHttpApiHostMigrationsDbContext(options);
        await migrationsDbContext.Database.MigrateAsync();
    }
}
