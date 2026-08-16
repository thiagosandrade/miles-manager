using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;

public sealed class GetRedemptionsHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetRedemptionsQuery, Result<IReadOnlyList<DTOs.FlightRedemptionDto>>>
{
    public async Task<Result<IReadOnlyList<DTOs.FlightRedemptionDto>>> Handle(GetRedemptionsQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var items = await context.FlightRedemptions
            .Where(x => x.UserId == loggedUserId)
            .AsNoTracking()
            .ProjectTo<DTOs.FlightRedemptionDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DTOs.FlightRedemptionDto>>.Success(items);
    }
}
