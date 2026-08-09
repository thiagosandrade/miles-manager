#nullable enable
using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Redemptions;

public class FlightRedemption : Entity
{
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public string OriginAirportCode { get; set; } = string.Empty;
    public string DestinationAirportCode { get; set; } = string.Empty;
    public DateOnly FlightDate { get; set; }
    public CabinClass CabinClass { get; set; }
    public int Passengers { get; set; }
    public bool IsRoundTrip { get; set; }
    public decimal MilesUsed { get; set; }
    public decimal Taxes { get; set; }
    public decimal CashPrice { get; set; }
    public decimal EffectiveCostPerThousand { get; set; }

    public virtual MilesManager.Domain.Programs.LoyaltyProgram LoyaltyProgram { get; set; } = null!;
}
