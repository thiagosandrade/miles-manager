#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Redemptions.DTOs;

namespace MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;

public sealed record GetRedemptionsQuery()
    : IRequest<Result<IReadOnlyList<FlightRedemptionDto>>>;
