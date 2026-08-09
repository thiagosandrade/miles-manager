#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Redemptions.Commands.UpdateFlightRedemption;

public sealed class UpdateFlightRedemptionCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ProgramId { get; init; }
    public string OriginAirportCode { get; init; } = string.Empty;
    public string DestinationAirportCode { get; init; } = string.Empty;
    public DateOnly FlightDate { get; init; }
    public CabinClass CabinClass { get; init; }
    public int Passengers { get; init; }
    public bool IsRoundTrip { get; init; }
    public decimal MilesUsed { get; init; }
    public decimal Taxes { get; init; }
    public decimal CashPrice { get; init; }
}
