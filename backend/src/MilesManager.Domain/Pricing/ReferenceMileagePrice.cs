#nullable enable
using MilesManager.Domain.Common;
using System;

namespace MilesManager.Domain.Pricing;

public class ReferenceMileagePrice : Entity
{
    public Guid ProgramId { get; set; }
    public decimal PricePerThousand { get; set; }
    public DateOnly EffectiveDate { get; set; }

    public virtual MilesManager.Domain.Programs.Program Program { get; set; } = null!;
}
