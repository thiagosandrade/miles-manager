#nullable enable
using MilesManager.Domain.Common;
using System;

namespace MilesManager.Domain.Transfers;

public class MileageTransfer : Entity
{
    public Guid UserId { get; set; }
    public Guid OriginProgramId { get; set; }
    public Guid DestinationProgramId { get; set; }
    public decimal MilesSent { get; set; }
    public decimal BonusPercentage { get; set; }
    public decimal MilesReceived { get; set; }
    public decimal CostPerThousandOrigin { get; set; }
    public decimal CostPerThousandDestination { get; set; }
    public DateOnly TransferDate { get; set; }

    public virtual MilesManager.Domain.Programs.Program OriginProgram { get; set; } = null!;
    public virtual MilesManager.Domain.Programs.Program DestinationProgram { get; set; } = null!;
}
