namespace MilesManager.Domain.Trips;

public sealed class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal DistanceMiles { get; set; }
    public DateOnly TripDate { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
