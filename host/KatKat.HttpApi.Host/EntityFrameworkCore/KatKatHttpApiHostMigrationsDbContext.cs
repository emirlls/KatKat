using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore;

/// <summary>
/// Shared migrations DbContext for this Host: KatKat's own tables plus the ABP infrastructure
/// tables actually needed here (Tenant/Permission/Setting/Identity/OpenIddict). AuditLogging is
/// intentionally NOT configured - its tables (AbpAuditLogs, AbpEntityChanges...) are disabled;
/// see AbpAuditingOptions.IsEnabled in KatKatHttpApiHostModule. Identity + OpenIddict are
/// configured because this Host is self-contained: it issues its own tokens (see
/// KatKatHttpApiHostModule's OpenIddict module dependencies and Program's AddAbpJwtBearer
/// Authority, which now points at this same Host) instead of trusting an external AuthServer.
/// </summary>
public class KatKatHttpApiHostMigrationsDbContext : AbpDbContext<KatKatHttpApiHostMigrationsDbContext>
{
    public KatKatHttpApiHostMigrationsDbContext(DbContextOptions<KatKatHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureKatKat();
        modelBuilder.ConfigureTenantManagement();
        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureIdentity();
        modelBuilder.ConfigureOpenIddict();
    }
}
