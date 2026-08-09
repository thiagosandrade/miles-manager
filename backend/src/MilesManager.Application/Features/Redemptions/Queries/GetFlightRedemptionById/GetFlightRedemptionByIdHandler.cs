using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Queries.GetFlightRedemptionById;

public sealed class GetFlightRedemptionByIdHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetFlightRedemptionByIdQuery, Result<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>> Handle(GetFlightRedemptionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.FlightRedemptions.AsNoTracking()
            .Where(r => r.Id == request.Id)
            .ProjectTo<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>.NotFound();
        }

        return Result<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>.Success(item);
    }
}
