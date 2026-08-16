using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;

public sealed class GetMileagePurchaseByIdHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetMileagePurchaseByIdQuery, Result<DTOs.MileagePurchaseDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<DTOs.MileagePurchaseDto>> Handle(GetMileagePurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var item = await _context.MileagePurchases.AsNoTracking()
            .Where(p => p.Id == request.Id && p.UserId == loggedUserId)
            .ProjectTo<DTOs.MileagePurchaseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<DTOs.MileagePurchaseDto>.NotFound();
        }

        return Result<DTOs.MileagePurchaseDto>.Success(item);
    }
}
