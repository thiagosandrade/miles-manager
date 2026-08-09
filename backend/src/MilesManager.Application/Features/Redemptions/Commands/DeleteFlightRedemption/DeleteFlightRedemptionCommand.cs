#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Redemptions.Commands.DeleteFlightRedemption;

public sealed class DeleteFlightRedemptionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
