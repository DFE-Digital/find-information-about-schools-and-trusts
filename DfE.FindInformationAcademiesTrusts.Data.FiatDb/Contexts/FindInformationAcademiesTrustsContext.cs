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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureWatchlist(modelBuilder);
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
        var (name, _) = userDetailsProvider.GetUserDetails();
        var utcNow = DateTime.UtcNow;

        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            entity.LastModifiedOn = utcNow;
            entity.LastModifiedBy = name;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedOn = utcNow;
                entity.CreatedBy = name;
            }
        }
    }
}
