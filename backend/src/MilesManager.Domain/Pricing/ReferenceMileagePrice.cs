#nullable enable
using MilesManager.Domain.Common;

namespace MilesManager.Domain.Pricing;

public class ReferenceMileagePrice : Entity
{
    public Guid ProgramId { get; set; }
    public decimal PricePerThousand { get; set; }
    public DateOnly EffectiveDate { get; set; }

    public virtual Programs.LoyaltyProgram Program { get; set; } = null!;
}
