using MilesManager.Application.Abstractions;
using MilesManager.Domain.Trips;
using MilesManager.Domain.Users;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Infrastructure.Persistence;

public sealed class MilesManagerDbContext(DbContextOptions<MilesManagerDbContext> options) : DbContext(options), IUnitOfWork, IApplicationDbContext
{
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<LoyaltyProgram> Programs => Set<LoyaltyProgram>();
    public DbSet<ProgramBalance> ProgramBalances => Set<ProgramBalance>();
    public DbSet<MileagePurchase> MileagePurchases => Set<MileagePurchase>();
    public DbSet<ProgramFactor> TransferRoutes => Set<ProgramFactor>();
    public DbSet<MileageTransfer> MileageTransfers => Set<MileageTransfer>();
    public DbSet<FlightRedemption> FlightRedemptions => Set<FlightRedemption>();
    public DbSet<ReferenceMileagePrice> ReferenceMileagePrices => Set<ReferenceMileagePrice>();
    public DbSet<Airport> Airports => Set<Airport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from this assembly (Infrastructure.Persistence.Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MilesManagerDbContext).Assembly);
    }
}
