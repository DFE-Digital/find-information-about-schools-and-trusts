using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Domain.Common;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;

public class FindInformationAcademiesTrustsContext(
    DbContextOptions<FindInformationAcademiesTrustsContext> options,
    IUserDetailsProvider userDetailsProvider)
    : DbContext(options)
{
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<SchoolContact> SchoolContacts { get; set; }
    public DbSet<TrustContact> TrustContacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureWatchlist(modelBuilder);
        ConfigureSchoolContact(modelBuilder);
        ConfigureTrustContact(modelBuilder);
    }

    private static void ConfigureWatchlist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.ToTable("Watchlist");
            entity.HasKey(w => w.Id);

            entity.Property(w => w.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new WatchlistId(value));

            entity.Property(w => w.ReadableId)
                .UseIdentityColumn();

            entity.Property(w => w.EstablishmentId).HasMaxLength(20);
            entity.Property(w => w.TrustId).HasMaxLength(20);
            entity.Property(w => w.User).HasMaxLength(320);
            entity.Property(w => w.CreatedBy).HasMaxLength(320);
            entity.Property(w => w.LastModifiedBy).HasMaxLength(320);

            entity.HasIndex(w => new { w.User, w.EstablishmentId });
            entity.HasIndex(w => new { w.User, w.TrustId });
        });
    }

    private static void ConfigureSchoolContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchoolContact>(entity =>
        {
            entity.ToTable("SchoolContacts", table => table.IsTemporal());
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Role).HasConversion<string>();
            entity.Property(c => c.Name).HasMaxLength(500);
            entity.Property(c => c.Email).HasMaxLength(320);
            entity.Property(c => c.LastModifiedByName).HasMaxLength(500);
            entity.Property(c => c.LastModifiedByEmail).HasMaxLength(320);
            entity.Property(c => c.LastModifiedAtTime).HasComputedColumnSql("[PeriodStart]");

            entity.HasIndex(c => c.Urn);
            entity.HasIndex(c => new { c.Urn, c.Role }).IsUnique();
        });
    }

    private static void ConfigureTrustContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrustContact>(entity =>
        {
            entity.ToTable("Contacts", table => table.IsTemporal());
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Role).HasConversion<string>();
            entity.Property(c => c.Name).HasMaxLength(500);
            entity.Property(c => c.Email).HasMaxLength(320);
            entity.Property(c => c.LastModifiedByName).HasMaxLength(500);
            entity.Property(c => c.LastModifiedByEmail).HasMaxLength(320);
            entity.Property(c => c.LastModifiedAtTime).HasComputedColumnSql("[PeriodStart]");

            entity.HasIndex(c => c.Uid);
            entity.HasIndex(c => new { c.Uid, c.Role }).IsUnique();
        });
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var (name, email) = userDetailsProvider.GetUserDetails();
        var utcNow = DateTime.UtcNow;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            switch (entry.Entity)
            {
                case IAuditableEntity entity:
                    entity.LastModifiedOn = utcNow;
                    entity.LastModifiedBy = name;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedOn = utcNow;
                        entity.CreatedBy = name;
                    }

                    break;

                case BaseEntity entity:
                    entity.LastModifiedByName = name;
                    entity.LastModifiedByEmail = email;
                    break;
            }
        }
    }
}
