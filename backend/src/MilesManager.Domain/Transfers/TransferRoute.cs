#nullable enable
using MilesManager.Domain.Common;

namespace MilesManager.Domain.Transfers;

public class TransferRoute : Entity
{
    public Guid OriginProgramId { get; set; }
    public Guid DestinationProgramId { get; set; }
    public decimal TransferFactor { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual MilesManager.Domain.Programs.LoyaltyProgram OriginProgram { get; set; } = null!;
    public virtual MilesManager.Domain.Programs.LoyaltyProgram DestinationProgram { get; set; } = null!;
}
