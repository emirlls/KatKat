using KatKat.Buildings;
using KatKat.Complexes;
using KatKat.FlatMembers;
using KatKat.Flats;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore;

[ConnectionStringName(KatKatDbProperties.ConnectionStringName)]
public class KatKatDbContext : AbpDbContext<KatKatDbContext>, IKatKatDbContext
{
    public DbSet<Complex> Complexes { get; set; } = null!;

    public DbSet<Building> Buildings { get; set; } = null!;

    public DbSet<Flat> Flats { get; set; } = null!;

    public DbSet<FlatMember> FlatMembers { get; set; } = null!;

    public KatKatDbContext(DbContextOptions<KatKatDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureKatKat();
    }
}
