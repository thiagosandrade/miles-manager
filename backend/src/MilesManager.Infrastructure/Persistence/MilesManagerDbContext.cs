using MilesManager.Application.Abstractions;
using MilesManager.Domain.Trips;
using MilesManager.Domain.Users;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Lookups;
using MilesManager.Domain.Alerts;
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
    public DbSet<AlertSource> AlertSources => Set<AlertSource>();
    public DbSet<AlertItem> AlertItems => Set<AlertItem>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var sources = await AlertSources
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.Country))
            .Select(x => new { x.ExternalUrl, x.Country })
            .ToListAsync(cancellationToken);

        foreach (var item in ChangeTracker.Entries<AlertItem>()
                     .Select(x => x.Entity)
                     .Where(x => string.IsNullOrWhiteSpace(x.Country)))
        {
            if (!Uri.TryCreate(item.ExternalUrl, UriKind.Absolute, out var itemUri))
                continue;

            var source = sources.FirstOrDefault(x =>
                Uri.TryCreate(x.ExternalUrl, UriKind.Absolute, out var sourceUri) &&
                string.Equals(sourceUri.Host, itemUri.Host, StringComparison.OrdinalIgnoreCase));

            if (source is not null)
                item.Country = source.Country;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MilesManagerDbContext).Assembly);
    }
}
