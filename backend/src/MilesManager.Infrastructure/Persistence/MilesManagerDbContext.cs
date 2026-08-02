using MilesManager.Application.Features.Trips.Abstractions;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Domain.Trips;
using MilesManager.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Infrastructure.Persistence;

public sealed class MilesManagerDbContext(DbContextOptions<MilesManagerDbContext> options) : DbContext(options), IUnitOfWork, IUserUnitOfWork
{
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<UserPreferences> UserPreferences => Set<UserPreferences>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("trips");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.DistanceMiles).HasPrecision(10, 2);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.Property(x => x.TokenVersion).HasDefaultValue(0);
        });

        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.ToTable("user_preferences");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.Property(x => x.PreferredCurrency).HasMaxLength(3).IsRequired();
            entity.Property(x => x.Theme).HasMaxLength(24);
        });
    }
}
