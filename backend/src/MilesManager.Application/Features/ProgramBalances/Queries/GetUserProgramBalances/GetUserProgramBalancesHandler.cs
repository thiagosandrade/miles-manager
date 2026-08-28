using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramBalances;

public sealed class GetUserProgramBalancesHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserProgramBalancesQuery, Result<IReadOnlyList<UserProgramBalancesDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<IReadOnlyList<UserProgramBalancesDto>>> Handle(
        GetUserProgramBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var purchases = await _context.MileagePurchases
            .AsNoTracking()
            .Where(p =>
                p.UserId == request.UserId &&
                p.PurchaseStatus == PurchaseStatus.Confirmed)
            .GroupBy(p => p.ProgramId)
            .Select(g => new
            {
                ProgramId = g.Key,
                TotalMilesPurchased = g.Sum(p => p.MilesPurchased),
                TotalAmountPaid = g.Sum(p => p.AmountPaid)
            })
            .ToDictionaryAsync(x => x.ProgramId, cancellationToken);

        var latestReferencePrices = await _context.ReferenceMileagePrices
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
            .ToDictionaryAsync(x => x.ProgramId, x => x.PricePerThousand, cancellationToken);

        var items = await _context.ProgramBalances
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .Include(x => x.Program)
            .Select(x => new
            {
                x.ProgramId,
                ProgramName = x.Program.Name,
                x.Balance,
                x.Program.CurrencyCode
            })
            .ToListAsync(cancellationToken);

        var result = items
            .Select(x =>
            {
                purchases.TryGetValue(x.ProgramId, out var purchaseStats);

                var totalMilesPurchased = purchaseStats?.TotalMilesPurchased ?? 0m;
                var totalAmountPaid = purchaseStats?.TotalAmountPaid ?? 0m;

                var actualCostPerThousand = totalMilesPurchased > 0m
                    ? totalAmountPaid / (totalMilesPurchased / 1000m)
                    : 0m;

                return new UserProgramBalancesDto
                {
                    ProgramId = x.ProgramId,
                    ProgramName = x.ProgramName,
                    Balance = x.Balance,
                    CurrentValue = actualCostPerThousand,
                    CurrencyCode = x.CurrencyCode
                };
            })
            .OrderBy(x => x.ProgramName)
            .ToList();

        return Result<IReadOnlyList<UserProgramBalancesDto>>.Success(result);
    }
}
