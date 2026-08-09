#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.DTOs;

namespace MilesManager.Application.Features.Transfers.Queries.GetTransfers;

public sealed class GetTransfersQuery : IRequest<Result<IReadOnlyList<MileageTransferDto>>>
{
}
