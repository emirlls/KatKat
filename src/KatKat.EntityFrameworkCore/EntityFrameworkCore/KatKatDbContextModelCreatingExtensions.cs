using KatKat.Buildings;
using KatKat.Complexes;
using KatKat.FlatMembers;
using KatKat.Flats;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace KatKat.EntityFrameworkCore;

public static class KatKatDbContextModelCreatingExtensions
{
    public static void ConfigureKatKat(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Complex>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + "Complexes", KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(ComplexConsts.MaxNameLength);
            b.Property(x => x.City).IsRequired().HasMaxLength(ComplexConsts.MaxCityLength);
            b.Property(x => x.District).IsRequired().HasMaxLength(ComplexConsts.MaxDistrictLength);
            b.Property(x => x.Address).HasMaxLength(ComplexConsts.MaxAddressLength);

            b.HasIndex(x => new { x.TenantId, x.Name });
        });

        builder.Entity<Building>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + "Buildings", KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(BuildingConsts.MaxNameLength);

            b.HasOne<Complex>().WithMany().HasForeignKey(x => x.ComplexId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.ComplexId, x.Name });
        });

        builder.Entity<Flat>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + "Flats", KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.FlatNumber).IsRequired().HasMaxLength(FlatConsts.MaxFlatNumberLength);
            b.Property(x => x.ShareFactor).HasPrecision(FlatConsts.ShareFactorPrecision, FlatConsts.ShareFactorScale);

            b.HasOne<Building>().WithMany().HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.BuildingId, x.FlatNumber });
        });

        builder.Entity<FlatMember>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + "FlatMembers", KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            // No FK constraint to the Identity module's AbpUsers table by design - IdentityUser
            // lives in a separate module/bounded context, so UserId is a loose reference.
            b.HasIndex(x => new { x.FlatId, x.UserId }).IsUnique();
            b.HasIndex(x => x.UserId);
        });
    }
}
