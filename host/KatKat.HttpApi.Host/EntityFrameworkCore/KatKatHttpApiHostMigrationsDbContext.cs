using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore;

/// <summary>
/// Shared migrations DbContext for this Host: KatKat's own tables plus the ABP infrastructure
/// tables actually needed here (Tenant/Permission/Setting management). AuditLogging is
/// intentionally NOT configured - its tables (AbpAuditLogs, AbpEntityChanges...) are disabled;
/// see AbpAuditingOptions.IsEnabled in KatKatHttpApiHostModule. Identity (Users/Roles) is not
/// configured either: this Host validates JWTs issued by a separate Authorization Server and
/// does not own user accounts itself.
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
    }
}
