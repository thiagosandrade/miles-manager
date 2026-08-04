#nullable enable
using MilesManager.Domain.Common;

namespace MilesManager.Domain.Lookups;

public class Airport : Entity
{
    // IATA code (e.g., "JFK") - intended as the primary key in the DB mapping
    public string IataCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
