using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Queries.GetFlightRedemptionById;

public sealed class GetFlightRedemptionByIdHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetFlightRedemptionByIdQuery, Result<DTOs.FlightRedemptionDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<DTOs.FlightRedemptionDto>> Handle(GetFlightRedemptionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.FlightRedemptions.AsNoTracking()
            .Where(r => r.Id == request.Id)
            .ProjectTo<DTOs.FlightRedemptionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<DTOs.FlightRedemptionDto>.NotFound();
        }

        return Result<DTOs.FlightRedemptionDto>.Success(item);
    }
}
