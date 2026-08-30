namespace MilesManager.Application.Features.ProgramFactors.DTOs;

public sealed class ProgramFactorDto
{
    public Guid Id { get; init; }
    public Guid OriginProgramId { get; init; }
    public string OriginProgramName { get; init; } = string.Empty;
    public Guid DestinationProgramId { get; init; }
    public string DestinationProgramName { get; init; } = string.Empty;
    public decimal TransferFactor { get; init; }
}
