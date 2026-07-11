using KatKat.Entities;
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

    public DbSet<P2PRequest> P2PRequests { get; set; } = null!;

    public DbSet<UserPreference> UserPreferences { get; set; } = null!;

    public DbSet<ComplexScore> ComplexScores { get; set; } = null!;

    public DbSet<Expense> Expenses { get; set; } = null!;

    public DbSet<ExpenseShare> ExpenseShares { get; set; } = null!;

    public DbSet<Issue> Issues { get; set; } = null!;

    public DbSet<Resource> Resources { get; set; } = null!;

    public DbSet<ResourceReservation> ResourceReservations { get; set; } = null!;

    public DbSet<SosAlert> SosAlerts { get; set; } = null!;

    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<District> Districts { get; set; } = null!;

    public DbSet<Neighborhood> Neighborhoods { get; set; } = null!;

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
