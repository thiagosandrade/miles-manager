using MediatR;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetDashboardQuery, DTOs.DashboardDto>
{
    public async Task<DTOs.DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var userLoggedId = currentUser.UserId;

        var balances = await context.ProgramBalances.Where(x => x.UserId == userLoggedId).AsNoTracking().ToListAsync(cancellationToken);
        var totalMiles = balances.Sum(b => b.Balance);

        var totalInvestment = await context.MileagePurchases.Where(x => x.UserId == userLoggedId).AsNoTracking().SumAsync(p => p.AmountPaid, cancellationToken);
        var averageCostPerThousand = totalMiles > 0 ? totalInvestment / (totalMiles / 1000m) : 0m;

        var prices = await context.ReferenceMileagePrices.AsNoTracking().ToListAsync(cancellationToken);
        var latestPriceByProgram = prices.GroupBy(p => p.ProgramId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.EffectiveDate).First().PricePerThousand);

        decimal currentMarketValue = 0m;
        foreach (var b in balances)
        {
            if (latestPriceByProgram.TryGetValue(b.ProgramId, out var pricePerThousand))
            {
                currentMarketValue += b.Balance * (pricePerThousand / 1000m);
            }
        }

        var profit = currentMarketValue - totalInvestment;
        var currentProfit = profit > 0 ? profit : 0m;
        var currentLoss = profit < 0 ? -profit : 0m;

        var programs = await context.ProgramBalances.Where(x => x.UserId == userLoggedId).AsNoTracking()
            .ProjectTo<Programs.DTOs.ProgramBalanceDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var recentPurchases = (await context.MileagePurchases.Where(x => x.UserId == userLoggedId).AsNoTracking()
            .OrderByDescending(p => p.PurchaseDate)
            .ProjectTo<Purchases.DTOs.MileagePurchaseDto>(mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var recentTransfers = (await context.MileageTransfers.Where(x => x.UserId == userLoggedId).AsNoTracking()
            .OrderByDescending(t => t.TransferDate)
            .ProjectTo<Transfers.DTOs.MileageTransferDto>(mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var recentRedemptions = (await context.FlightRedemptions.Where(x => x.UserId == userLoggedId).AsNoTracking()
            .OrderByDescending(r => r.FlightDate)
            .ProjectTo<Redemptions.DTOs.FlightRedemptionDto>(mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var dto = new DTOs.DashboardDto
        {
            TotalMiles = totalMiles,
            TotalInvestment = totalInvestment,
            AverageCostPerThousand = averageCostPerThousand,
            CurrentMarketValue = currentMarketValue,
            CurrentProfit = currentProfit,
            CurrentLoss = currentLoss,
            Programs = programs.Cast<object>().ToList(),
            RecentPurchases = recentPurchases,
            RecentTransfers = recentTransfers,
            RecentRedemptions = recentRedemptions
        };

        return dto;
    }
}
