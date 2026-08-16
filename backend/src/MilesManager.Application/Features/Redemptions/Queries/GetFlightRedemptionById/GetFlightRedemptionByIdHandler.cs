using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Redemptions.Queries.GetFlightRedemptionById;

public sealed class GetFlightRedemptionByIdHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetFlightRedemptionByIdQuery, Result<DTOs.FlightRedemptionDto>>
{
    public async Task<Result<DTOs.FlightRedemptionDto>> Handle(GetFlightRedemptionByIdQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var item = await context.FlightRedemptions
            .AsNoTracking()
            .Where(r => r.Id == request.Id && r.UserId == loggedUserId)
            .ProjectTo<DTOs.FlightRedemptionDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<DTOs.FlightRedemptionDto>.NotFound();
        }

        return Result<DTOs.FlightRedemptionDto>.Success(item);
    }
}
