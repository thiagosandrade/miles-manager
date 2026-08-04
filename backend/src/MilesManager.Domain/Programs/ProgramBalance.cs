#nullable enable
using MilesManager.Domain.Common;
using System;

namespace MilesManager.Domain.Programs;

public class ProgramBalance : Entity
{
    public Guid UserId { get; set; }
    public Guid ProgramId { get; set; }
    public decimal Balance { get; set; }

    public virtual Program Program { get; set; } = null!;
}
