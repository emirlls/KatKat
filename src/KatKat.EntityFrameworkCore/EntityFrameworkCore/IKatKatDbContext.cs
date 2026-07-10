using KatKat.Buildings;
using KatKat.Complexes;
using KatKat.FlatMembers;
using KatKat.Flats;
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
}
