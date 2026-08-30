#nullable enable
using MilesManager.Domain.Common;

namespace MilesManager.Domain.Transfers;

public class TransferRoute : Entity
{
    public Guid OriginProgramId { get; set; }
    public Guid DestinationProgramId { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }

    public virtual Programs.LoyaltyProgram OriginProgram { get; set; } = null!;
    public virtual Programs.LoyaltyProgram DestinationProgram { get; set; } = null!;
}
