using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Context;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUser currentUser)
    : IRequestHandler<GetDashboardQuery, DTOs.DashboardDto>
{
    public async Task<DTOs.DashboardDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        // =========================================================
        // Current balances
        // =========================================================

        var balances = await context.ProgramBalances
            .Where(x => x.UserId == userId)
            .Include(x => x.Program)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // =========================================================
        // Confirmed purchases
        // =========================================================

        var purchases = await context.MileagePurchases
            .Where(x =>
                x.UserId == userId &&
                x.PurchaseStatus == PurchaseStatus.Confirmed)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // =========================================================
        // Latest reference price for each program
        // =========================================================

        var latestReferencePrices = await context.ReferenceMileagePrices
            .AsNoTracking()
            .GroupBy(x => x.ProgramId)
            .Select(g => g
                .OrderByDescending(x => x.EffectiveDate)
                .Select(x => new
                {
                    x.ProgramId,
                    x.PricePerThousand
                })
                .First())
            .ToDictionaryAsync(
                x => x.ProgramId,
                x => x.PricePerThousand,
                cancellationToken);

        // =========================================================
        // Purchase statistics by program
        // =========================================================

        var purchaseStatsByProgram = purchases
            .GroupBy(x => x.ProgramId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalAmount = g.Sum(x => x.AmountPaid),
                    TotalMiles = g.Sum(x => x.MilesPurchased)
                });

        // =========================================================
        // Program dashboard data
        // =========================================================

        var programs = balances
            .Select(balance =>
            {
                var program = balance.Program;

                var hasPurchaseData = purchaseStatsByProgram.TryGetValue(program.Id, out var purchaseStats);

                var totalPurchaseAmount = hasPurchaseData && purchaseStats != null
                        ? purchaseStats.TotalAmount
                        : 0m;

                var totalPurchasedMiles = hasPurchaseData && purchaseStats != null
                        ? purchaseStats.TotalMiles
                        : 0m;

                var actualCostPerThousand = totalPurchasedMiles > 0
                        ? totalPurchaseAmount /
                          (totalPurchasedMiles / 1000m)
                        : 0m;

                var referencePricePerThousand = latestReferencePrices.TryGetValue(program.Id, out var referencePrice)
                        ? referencePrice
                        : 0m;

                var unrealizedProfitLoss = (referencePricePerThousand - actualCostPerThousand) * (balance.Balance / 1000m);

                var unrealizedProfitLossPercentage = actualCostPerThousand > 0
                        ? (
                            (referencePricePerThousand -
                             actualCostPerThousand)
                            / actualCostPerThousand
                          ) * 100m
                        : 0m;

                return new DTOs.DashboardProgramDto
                {
                    ProgramId = program.Id,
                    ProgramName = program.Name,
                    CurrencyCode = program.CurrencyCode,
                    Miles = balance.Balance,
                    ActualCostPerThousand = actualCostPerThousand,
                    ReferencePricePerThousand = referencePricePerThousand,
                    UnrealizedProfitLoss = unrealizedProfitLoss,
                    UnrealizedProfitLossPercentage = unrealizedProfitLossPercentage
                };
            })
            .OrderBy(x => x.ProgramName)
            .ToList();

        // =========================================================
        // Recent purchases
        // =========================================================

        var recentPurchases =
            (await context.MileagePurchases
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(x => x.PurchaseDate)
                .ProjectTo<Purchases.DTOs.MileagePurchaseDto>(
                    mapper.ConfigurationProvider)
                .Take(5)
                .ToListAsync(cancellationToken))
            .Cast<object>()
            .ToList();

        // =========================================================
        // Recent transfers
        // =========================================================

        var recentTransfers =
            (await context.MileageTransfers
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(x => x.TransferDate)
                .ProjectTo<Transfers.DTOs.MileageTransferDto>(
                    mapper.ConfigurationProvider)
                .Take(5)
                .ToListAsync(cancellationToken))
            .Cast<object>()
            .ToList();

        // =========================================================
        // Recent redemptions
        // =========================================================

        var recentRedemptions =
            (await context.FlightRedemptions
                .Include(x => x.LoyaltyProgram)
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(x => x.FlightDate)
                .ProjectTo<Redemptions.DTOs.FlightRedemptionDto>(
                    mapper.ConfigurationProvider)
                .Take(5)
                .ToListAsync(cancellationToken))
            .Cast<object>()
            .ToList();

        // =========================================================
        // Dashboard
        // =========================================================

        return new DTOs.DashboardDto
        {
            ProgramCount = programs.Count,
            Programs = programs,
            RecentPurchases = recentPurchases,
            RecentTransfers = recentTransfers,
            RecentRedemptions = recentRedemptions
        };
    }
}
