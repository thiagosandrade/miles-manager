#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.DTOs;

namespace MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;

public sealed class GetMileageTransferByIdQuery : IRequest<Result<MileageTransferDto>>
{
    public Guid Id { get; init; }
}
