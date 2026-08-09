#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Purchases.Commands.DeleteMileagePurchase;

public sealed class DeleteMileagePurchaseCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
