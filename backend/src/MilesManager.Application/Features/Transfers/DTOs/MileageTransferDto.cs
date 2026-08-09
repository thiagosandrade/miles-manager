#nullable enable
namespace MilesManager.Application.Features.Transfers.DTOs;

public sealed class MileageTransferDto
{
    public Guid Id { get; init; }
    public string OriginProgram { get; init; } = string.Empty;
    public string DestinationProgram { get; init; } = string.Empty;
    public decimal MilesSent { get; init; }
    public decimal BonusPercentage { get; init; }
    public decimal MilesReceived { get; init; }
    public DateOnly TransferDate { get; init; }
    public decimal CostPerThousandOrigin { get; init; }
    public decimal CostPerThousandDestination { get; init; }
}
