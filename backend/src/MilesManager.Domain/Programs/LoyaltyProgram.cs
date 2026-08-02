using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Programs;

public sealed class LoyaltyProgram : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid PartnerId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public ProgramStatus Status { get; set; } = ProgramStatus.Active;
    public int ExpirationAfterMonths { get; set; } = 36;
    public bool ExpirationEnabled { get; set; } = true;
}
