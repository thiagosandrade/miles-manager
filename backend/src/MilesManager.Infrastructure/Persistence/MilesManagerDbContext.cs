using MilesManager.Application.Abstractions;
using MilesManager.Domain.Trips;
using MilesManager.Domain.Users;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using MilesManager.Domain.Partners;
using MilesManager.Domain.Accounts;
using MilesManager.Domain.Transactions;

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
    public DbSet<TransferRoute> TransferRoutes => Set<TransferRoute>();
    public DbSet<MileageTransfer> MileageTransfers => Set<MileageTransfer>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<MileageAccount> MileageAccounts => Set<MileageAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<EarnTransaction> EarnTransactions => Set<EarnTransaction>();
    public DbSet<ExpirationTransaction> ExpirationTransactions => Set<ExpirationTransaction>();
    public DbSet<RedeemTransaction> RedeemTransactions => Set<RedeemTransaction>();
    public DbSet<TransferTransaction> TransferTransactions => Set<TransferTransaction>();
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
