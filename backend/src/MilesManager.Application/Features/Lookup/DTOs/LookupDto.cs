#nullable enable
namespace MilesManager.Application.Features.Lookup.DTOs;

public sealed class LookupDto
{
    public IReadOnlyList<AirportDto> Airports { get; init; } = [];
}

public class AirportDto
{
    public string IataCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
}
