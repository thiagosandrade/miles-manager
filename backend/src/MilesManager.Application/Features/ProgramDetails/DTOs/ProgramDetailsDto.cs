#nullable enable

namespace MilesManager.Application.Features.ProgramDetails.DTOs;

public sealed class ProgramDetailsDto
{
    public Guid ProgramId { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public IReadOnlyList<ProgramActivityDto> Activity { get; init; } = [];
    public IReadOnlyList<ProgramRedemptionDto> Redemptions { get; init; } = [];
}

public sealed class ProgramActivityDto
{
    public DateOnly Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public decimal Miles { get; init; }
    public decimal Balance { get; init; }
}

public sealed class ProgramRedemptionDto
{
    public Guid Id { get; init; }
    public DateOnly FlightDate { get; init; }
    public string OriginAirportCode { get; init; } = string.Empty;
    public string DestinationAirportCode { get; init; } = string.Empty;
    public decimal MilesUsed { get; init; }
    public int Passengers { get; init; }
    public bool IsRoundTrip { get; init; }
}
