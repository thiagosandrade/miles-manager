using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;

public sealed class GetRedemptionsHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetRedemptionsQuery, Result<IReadOnlyList<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IReadOnlyList<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>>> Handle(GetRedemptionsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.FlightRedemptions.AsNoTracking()
            .ProjectTo<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>>.Success(items);
    }
}
