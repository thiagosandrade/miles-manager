using MediatR;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetDashboardQuery, MilesManager.Application.Features.Dashboard.DTOs.DashboardDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<MilesManager.Application.Features.Dashboard.DTOs.DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var balances = await _context.ProgramBalances.AsNoTracking().ToListAsync(cancellationToken);
        var totalMiles = balances.Sum(b => b.Balance);

        var totalInvestment = await _context.MileagePurchases.AsNoTracking().SumAsync(p => p.AmountPaid, cancellationToken);
        var averageCostPerThousand = totalMiles > 0 ? totalInvestment / (totalMiles / 1000m) : 0m;

        var prices = await _context.ReferenceMileagePrices.AsNoTracking().ToListAsync(cancellationToken);
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

        var programs = await _context.ProgramBalances.AsNoTracking()
            .ProjectTo<MilesManager.Application.Features.Programs.DTOs.ProgramBalanceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var recentPurchases = (await _context.MileagePurchases.AsNoTracking()
            .OrderByDescending(p => p.PurchaseDate)
            .ProjectTo<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>(_mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var recentTransfers = (await _context.MileageTransfers.AsNoTracking()
            .OrderByDescending(t => t.TransferDate)
            .ProjectTo<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>(_mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var recentRedemptions = (await _context.FlightRedemptions.AsNoTracking()
            .OrderByDescending(r => r.FlightDate)
            .ProjectTo<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>(_mapper.ConfigurationProvider)
            .Take(5)
            .ToListAsync(cancellationToken)).Cast<object>().ToList();

        var dto = new MilesManager.Application.Features.Dashboard.DTOs.DashboardDto
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
