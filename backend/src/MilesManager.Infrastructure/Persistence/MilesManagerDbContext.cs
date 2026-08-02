using MilesManager.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Trips.Abstractions;

namespace MilesManager.Infrastructure.Persistence;

public sealed class MilesManagerDbContext(DbContextOptions<MilesManagerDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("trips");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.DistanceMiles).HasPrecision(10, 2);
        });
    }
}
