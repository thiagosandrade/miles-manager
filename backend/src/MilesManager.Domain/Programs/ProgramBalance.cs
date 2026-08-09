#nullable enable
using MilesManager.Domain.Common;

namespace MilesManager.Domain.Programs;

public class ProgramBalance : Entity
{
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public decimal Balance { get; set; }

    public virtual LoyaltyProgram Program { get; set; } = null!;
}
