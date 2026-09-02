#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Transfers.Commands.CreateTransfer;

public sealed class CreateTransferCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public Guid OriginProgramId { get; init; }
    public Guid DestinationProgramId { get; init; }
    public decimal MilesSent { get; init; }
    public decimal BonusPercentage { get; init; }
    public decimal MilesReceived { get; init; }
    public decimal? FactorFrom { get; init; }
    public decimal? FactorTo { get; init; }
    public DateOnly TransferDate { get; init; }
}
