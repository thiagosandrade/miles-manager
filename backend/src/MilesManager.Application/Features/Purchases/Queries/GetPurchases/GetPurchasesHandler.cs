using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Purchases.Queries.GetPurchases;

public sealed class GetPurchasesHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetPurchasesQuery, Result<IReadOnlyList<DTOs.MileagePurchaseDto>>>
{
    public async Task<Result<IReadOnlyList<DTOs.MileagePurchaseDto>>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var items = await context.MileagePurchases
            .Where(x => x.UserId == loggedUserId)
            .AsNoTracking()
            .ProjectTo<DTOs.MileagePurchaseDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DTOs.MileagePurchaseDto>>.Success(items);
    }
}
