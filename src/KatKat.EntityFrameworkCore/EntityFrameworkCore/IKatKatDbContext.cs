using KatKat.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore;

[ConnectionStringName(KatKatDbProperties.ConnectionStringName)]
public interface IKatKatDbContext : IEfCoreDbContext
{
    DbSet<Complex> Complexes { get; }

    DbSet<Building> Buildings { get; }

    DbSet<Flat> Flats { get; }

    DbSet<FlatMember> FlatMembers { get; }

    DbSet<P2PRequest> P2PRequests { get; }

    DbSet<UserPreference> UserPreferences { get; }

    DbSet<ComplexScore> ComplexScores { get; }

    DbSet<Expense> Expenses { get; }

    DbSet<ExpenseShare> ExpenseShares { get; }

    DbSet<Issue> Issues { get; }

    DbSet<Resource> Resources { get; }

    DbSet<ResourceReservation> ResourceReservations { get; }

    DbSet<SosAlert> SosAlerts { get; }

    DbSet<City> Cities { get; }

    DbSet<District> Districts { get; }

    DbSet<Neighborhood> Neighborhoods { get; }
}
