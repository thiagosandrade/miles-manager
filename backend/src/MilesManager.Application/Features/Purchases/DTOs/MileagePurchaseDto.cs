#nullable enable
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Purchases.DTOs;

public sealed class MileagePurchaseDto
{
    public Guid Id { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public string? Partner { get; init; }
    public string? Description { get; init; }
    public decimal AmountPaid { get; init; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal MilesPurchased { get; init; }
    public decimal BonusPercentage { get; init; }
    public DateOnly PurchaseDate { get; init; }
    public PurchaseStatus PurchaseStatus { get; init; }
}
