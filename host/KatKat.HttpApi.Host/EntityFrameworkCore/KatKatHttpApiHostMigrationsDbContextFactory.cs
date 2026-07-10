using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KatKat.EntityFrameworkCore;

public class KatKatHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<KatKatHttpApiHostMigrationsDbContext>
{
    public KatKatHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<KatKatHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("KatKat"));

        return new KatKatHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
