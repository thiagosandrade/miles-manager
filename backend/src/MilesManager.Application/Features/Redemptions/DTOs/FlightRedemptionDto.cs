#nullable enable
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Redemptions.DTOs;

public sealed class FlightRedemptionDto
{
    public Guid Id { get; init; }
    public string Program { get; init; } = string.Empty;
    public string OriginAirportCode { get; init; } = string.Empty;
    public string DestinationAirportCode { get; init; } = string.Empty;
    public DateOnly FlightDate { get; init; }
    public CabinClass CabinClass { get; init; }
    public int Passengers { get; init; }
    public bool IsRoundTrip { get; init; }
    public decimal MilesUsed { get; init; }
    public decimal Taxes { get; init; }
    public decimal CashPrice { get; init; }
    public decimal EffectiveCostPerThousand { get; init; }
}
