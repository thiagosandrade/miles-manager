#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;

public sealed class UpdateMileageTransferCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid OriginProgramId { get; init; }
    public Guid DestinationProgramId { get; init; }
    public decimal MilesSent { get; init; }
    public decimal BonusPercentage { get; init; }
    public decimal MilesReceived { get; init; }
    public decimal CostPerThousandOrigin { get; init; }
    public decimal CostPerThousandDestination { get; init; }
    public DateOnly TransferDate { get; init; }
}
