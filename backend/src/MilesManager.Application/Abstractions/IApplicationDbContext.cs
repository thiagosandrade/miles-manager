using Microsoft.EntityFrameworkCore;
using MilesManager.Domain.Trips;
using MilesManager.Domain.Users;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Lookups;

namespace MilesManager.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Trip> Trips { get; }
    DbSet<AppUser> Users { get; }
    DbSet<MilesManager.Domain.Users.UserPreferences> UserPreferences { get; }
    DbSet<MilesManager.Domain.Programs.LoyaltyProgram> Programs { get; }
    DbSet<ProgramBalance> ProgramBalances { get; }
    DbSet<MileagePurchase> MileagePurchases { get; }
    DbSet<TransferRoute> TransferRoutes { get; }
    DbSet<MileageTransfer> MileageTransfers { get; }
    DbSet<FlightRedemption> FlightRedemptions { get; }
    DbSet<ReferenceMileagePrice> ReferenceMileagePrices { get; }
    DbSet<Airport> Airports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
