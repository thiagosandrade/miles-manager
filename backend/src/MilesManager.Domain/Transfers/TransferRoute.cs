#nullable enable
using MilesManager.Domain.Common;
using System;

namespace MilesManager.Domain.Transfers;

public class TransferRoute : Entity
{
    public Guid OriginProgramId { get; set; }
    public Guid DestinationProgramId { get; set; }
    public decimal TransferFactor { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual MilesManager.Domain.Programs.Program OriginProgram { get; set; } = null!;
    public virtual MilesManager.Domain.Programs.Program DestinationProgram { get; set; } = null!;
}
