using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;

namespace MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

public sealed class GetUserProgramPurchasesHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserProgramPurchasesQuery, Result<IReadOnlyList<UserProgramPurchaseDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<IReadOnlyList<UserProgramPurchaseDto>>> Handle(
        GetUserProgramPurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _context.Programs
            .AsNoTracking()
            .Select(program => new
            {
                program.Id,
                program.Name,
                program.CurrencyCode,
                TotalMilesPurchased = _context.MileagePurchases
                    .AsNoTracking()
                    .Where(p => p.UserId == request.UserId && p.ProgramId == program.Id)
                    .Select(p => (decimal?)p.MilesPurchased)
                    .Sum() ?? 0m,
                CurrentPricePerThousand = _context.ReferenceMileagePrices
                    .AsNoTracking()
                    .Where(r => r.ProgramId == program.Id)
                    .OrderByDescending(r => r.EffectiveDate)
                    .Select(r => (decimal?)r.PricePerThousand)
                    .FirstOrDefault() ?? program.ReferencePricePerThousand
            })
            .Select(x => new UserProgramPurchaseDto
            {
                ProgramId = x.Id,
                ProgramName = x.Name,
                Balance = x.TotalMilesPurchased,
                ReferencePricePerThousand = x.CurrentPricePerThousand,
                CurrentValue = (x.TotalMilesPurchased / 1000m) * x.CurrentPricePerThousand,
                CurrencyCode = x.CurrencyCode
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<UserProgramPurchaseDto>>.Success(items);
    }
}
