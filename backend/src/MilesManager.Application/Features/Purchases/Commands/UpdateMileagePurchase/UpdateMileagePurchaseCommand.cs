#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;

public sealed class UpdateMileagePurchaseCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ProgramId { get; init; }
    public string? Partner { get; init; }
    public string? Description { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal MilesPurchased { get; init; }
    public decimal BonusPercentage { get; init; }
    public DateOnly PurchaseDate { get; init; }
}
