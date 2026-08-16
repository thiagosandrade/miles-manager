using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;

public sealed class GetRedemptionsHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetRedemptionsQuery, Result<IReadOnlyList<DTOs.FlightRedemptionDto>>>
{
    public async Task<Result<IReadOnlyList<DTOs.FlightRedemptionDto>>> Handle(GetRedemptionsQuery request, CancellationToken cancellationToken)
    {
        var items = await context.FlightRedemptions.AsNoTracking()
            .ProjectTo<DTOs.FlightRedemptionDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DTOs.FlightRedemptionDto>>.Success(items);
    }
}
