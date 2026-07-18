using KatKat.Constants;
using KatKat.Entities;
using KatKat.P2PRequests;
using KatKat.Resources;
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
            b.ToTable(KatKatDbProperties.DbTablePrefix + ComplexConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(ComplexConsts.MaxNameLength);
            b.Property(x => x.Address).HasMaxLength(ComplexConsts.MaxAddressLength);
            b.Property(x => x.Latitude).HasPrecision(GeoConsts.CoordinatePrecision, GeoConsts.CoordinateScale);
            b.Property(x => x.Longitude).HasPrecision(GeoConsts.CoordinatePrecision, GeoConsts.CoordinateScale);

            b.HasOne<Neighborhood>().WithMany().HasForeignKey(x => x.NeighborhoodId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            b.HasIndex(x => new { x.TenantId, x.Name });
        });

        builder.Entity<City>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + CityConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(CityConsts.MaxNameLength);

            b.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<District>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + DistrictConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(DistrictConsts.MaxNameLength);

            b.HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            b.HasIndex(x => new { x.CityId, x.Name }).IsUnique();
        });

        builder.Entity<Neighborhood>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + NeighborhoodConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(NeighborhoodConsts.MaxNameLength);

            b.HasOne<District>().WithMany().HasForeignKey(x => x.DistrictId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            b.HasIndex(x => new { x.DistrictId, x.Name }).IsUnique();
        });

        builder.Entity<Building>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + BuildingConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(BuildingConsts.MaxNameLength);

            b.HasOne<Complex>().WithMany().HasForeignKey(x => x.ComplexId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.ComplexId, x.Name });
        });

        builder.Entity<Flat>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + FlatConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.FlatNumber).IsRequired().HasMaxLength(FlatConsts.MaxFlatNumberLength);
            b.Property(x => x.ShareFactor).HasPrecision(FlatConsts.ShareFactorPrecision, FlatConsts.ShareFactorScale);

            b.HasOne<Building>().WithMany().HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.BuildingId, x.FlatNumber });
        });

        builder.Entity<FlatMember>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + FlatMemberConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            // No FK constraint to the Identity module's AbpUsers table by design - IdentityUser
            // lives in a separate module/bounded context, so UserId is a loose reference.
            b.HasIndex(x => new { x.FlatId, x.UserId }).IsUnique();
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<P2PRequest>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + P2PRequestConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(P2PRequestConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(P2PRequestConsts.MaxDescriptionLength);

            // ComplexId is not FK-constrained: a Complex spans Buildings/Flats but a P2PRequest
            // is scoped to the whole Complex directly, and RequesterUserId/FulfilledByUserId are
            // loose references to the Identity module, same convention as FlatMember.UserId.
            b.HasIndex(x => new { x.ComplexId, x.Status });
            b.HasIndex(x => x.RequesterUserId);

            // SetNull, not Cascade - a deleted Flat shouldn't erase P2PRequest history, just its
            // Building/Flat attribution.
            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<UserPreference>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + UserPreferenceConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();
        });

        builder.Entity<ComplexScore>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ComplexScoreConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(ComplexConsts.MaxNameLength);
            b.Property(x => x.Latitude).HasPrecision(GeoConsts.CoordinatePrecision, GeoConsts.CoordinateScale);
            b.Property(x => x.Longitude).HasPrecision(GeoConsts.CoordinatePrecision, GeoConsts.CoordinateScale);
            b.Property(x => x.FinancialScore).HasPrecision(KatKatConsts.ScorePrecision, KatKatConsts.ScoreScale);
            b.Property(x => x.SocialScore).HasPrecision(KatKatConsts.ScorePrecision, KatKatConsts.ScoreScale);
            b.Property(x => x.ResolutionScore).HasPrecision(KatKatConsts.ScorePrecision, KatKatConsts.ScoreScale);
            b.Property(x => x.TotalScore).HasPrecision(KatKatConsts.ScorePrecision, KatKatConsts.ScoreScale);

            // Denormalized location FKs (see ComplexScore remarks) - Restrict, not Cascade, since
            // City/District/Neighborhood are shared admin-managed reference data, not owned dependents.
            b.HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict).IsRequired();
            b.HasOne<District>().WithMany().HasForeignKey(x => x.DistrictId).OnDelete(DeleteBehavior.Restrict).IsRequired();
            b.HasOne<Neighborhood>().WithMany().HasForeignKey(x => x.NeighborhoodId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            b.HasIndex(x => x.ComplexId).IsUnique();
            b.HasIndex(x => new { x.DistrictId, x.TotalScore });
            b.HasIndex(x => new { x.DistrictId, x.NeighborhoodId, x.TotalScore });
            // Bounding-box prefilter index for the nearby (radius-based) leaderboard.
            b.HasIndex(x => new { x.Latitude, x.Longitude });
        });

        builder.Entity<Expense>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ExpenseConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(ExpenseConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(ExpenseConsts.MaxDescriptionLength);
            b.Property(x => x.ReceiptImageUrl).HasMaxLength(ExpenseConsts.MaxReceiptImageUrlLength);
            b.Property(x => x.TotalAmount).HasPrecision(KatKatConsts.AmountPrecision, KatKatConsts.AmountScale);

            b.HasIndex(x => new { x.ComplexId, x.IssuedAt });
        });

        builder.Entity<ExpenseShare>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ExpenseShareConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Amount).HasPrecision(KatKatConsts.AmountPrecision, KatKatConsts.AmountScale);

            b.HasOne<Expense>().WithMany().HasForeignKey(x => x.ExpenseId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => x.FlatId);
            b.HasIndex(x => new { x.ExpenseId, x.IsPaid });
        });

        builder.Entity<Issue>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + IssueConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(IssueConsts.MaxTitleLength);
            b.Property(x => x.Description).HasMaxLength(IssueConsts.MaxDescriptionLength);
            b.Property(x => x.PhotoUrl).HasMaxLength(IssueConsts.MaxPhotoUrlLength);

            // SetNull, not Cascade - a deleted Building shouldn't erase Issue history, just its
            // Building attribution.
            b.HasOne<Building>().WithMany().HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => new { x.ComplexId, Status = x.Statuses });
            b.HasIndex(x => x.ReporterUserId);
            b.HasIndex(x => x.BuildingId);
        });

        builder.Entity<Resource>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ResourceConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(ResourceConsts.MaxNameLength);

            b.HasOne<Complex>().WithMany().HasForeignKey(x => x.ComplexId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.ComplexId, x.Type });
        });

        builder.Entity<ResourceReservation>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ResourceReservationConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasOne<Resource>().WithMany().HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            // SetNull, not Cascade - a deleted Flat shouldn't erase reservation history, just its
            // Building/Flat attribution.
            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.SetNull);

            // Overlap checks always filter by ResourceId + Status first, so that's the index shape.
            b.HasIndex(x => new { x.ResourceId, x.Status, x.StartTime, x.EndTime });
        });

        builder.Entity<SosAlert>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + SosAlertConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => new { x.ComplexId, x.CreationTime });
            b.HasIndex(x => x.FlatId);
        });

        builder.Entity<ResidentInvitation>(b =>
        {
            b.ToTable(KatKatDbProperties.DbTablePrefix + ResidentInvitationConsts.TableName, KatKatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(ResidentInvitationConsts.CodeLength);

            b.HasOne<Flat>().WithMany().HasForeignKey(x => x.FlatId).OnDelete(DeleteBehavior.Cascade).IsRequired();

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.FlatId);
        });
    }
}
