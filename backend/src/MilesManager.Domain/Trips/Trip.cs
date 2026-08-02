using MilesManager.Domain.Common;

namespace MilesManager.Domain.Trips;

public sealed class Trip : Entity
{
    public string Name { get; set; } = string.Empty;
    public decimal DistanceMiles { get; set; }
    public DateOnly TripDate { get; set; }
}
