#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;

public sealed class DeleteMileageTransferCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
