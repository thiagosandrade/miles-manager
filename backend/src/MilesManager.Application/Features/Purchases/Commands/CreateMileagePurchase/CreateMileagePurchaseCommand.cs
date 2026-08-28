#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;

public sealed class CreateMileagePurchaseCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public Guid ProgramId { get; init; }
    public string? Partner { get; init; }
    public string? Description { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal MilesPurchased { get; init; }
    public DateOnly PurchaseDate { get; init; }
}
