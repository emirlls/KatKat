using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore;

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
    }
}
