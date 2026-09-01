using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

namespace MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramBalances;

public sealed class GetUserProgramBalancesHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserProgramBalancesQuery, Result<IReadOnlyList<UserProgramBalancesDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<IReadOnlyList<UserProgramBalancesDto>>> Handle(GetUserProgramBalancesQuery request, CancellationToken cancellationToken)
    {
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

        var latestReferencePrices = await _context.ReferenceMileagePrices
            .AsNoTracking()
            .GroupBy(x => x.ProgramId)
            .Select(g => g.OrderByDescending(x => x.EffectiveDate).Select(x => new { x.ProgramId, x.PricePerThousand }).First())
            .ToDictionaryAsync(x => x.ProgramId, x => x.PricePerThousand, cancellationToken);

        var result = items
            .Select(x => new UserProgramBalancesDto
            {
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName,
                Balance = x.Balance,
                CurrentValue = latestReferencePrices.TryGetValue(x.ProgramId, out var pricePerThousand)
                    ? pricePerThousand * (x.Balance / 1000m)
                    : 0m,
                CurrencyCode = x.CurrencyCode
            })
            .OrderBy(x => x.ProgramName)
            .ToList();

        return Result<IReadOnlyList<UserProgramBalancesDto>>.Success(result);
    }
}