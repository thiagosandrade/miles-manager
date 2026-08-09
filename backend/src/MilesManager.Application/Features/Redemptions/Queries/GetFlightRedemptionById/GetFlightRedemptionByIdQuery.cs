#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Redemptions.DTOs;

namespace MilesManager.Application.Features.Redemptions.Queries.GetFlightRedemptionById;

public sealed class GetFlightRedemptionByIdQuery : IRequest<Result<FlightRedemptionDto>>
{
    public Guid Id { get; init; }
}
