using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Purchases.Queries.GetPurchases;

public sealed class GetPurchasesHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetPurchasesQuery, Result<IReadOnlyList<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IReadOnlyList<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.MileagePurchases.AsNoTracking()
            .ProjectTo<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>>.Success(items);
    }
}
