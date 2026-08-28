#nullable enable
using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Programs;

public class MileagePurchase : Entity
{
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public string? Partner { get; set; }
    public string? Description { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal MilesPurchased { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public PurchaseStatus PurchaseStatus { get; set; }

    public virtual LoyaltyProgram Program { get; set; } = null!;
}
