#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Redemptions.DTOs;

namespace MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;

public sealed class GetRedemptionsQuery : IRequest<Result<IReadOnlyList<FlightRedemptionDto>>>
{
}
