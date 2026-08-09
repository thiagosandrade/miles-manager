#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Purchases.DTOs;

namespace MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;

public sealed class GetMileagePurchaseByIdQuery : IRequest<Result<MileagePurchaseDto>>
{
    public Guid Id { get; init; }
}
