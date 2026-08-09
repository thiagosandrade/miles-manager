using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;

public sealed class GetMileagePurchaseByIdHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetMileagePurchaseByIdQuery, Result<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>> Handle(GetMileagePurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.MileagePurchases.AsNoTracking()
            .Where(p => p.Id == request.Id)
            .ProjectTo<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>.NotFound();
        }

        return Result<MilesManager.Application.Features.Purchases.DTOs.MileagePurchaseDto>.Success(item);
    }
}
