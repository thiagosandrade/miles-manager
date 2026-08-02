using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Transactions;

public abstract class Transaction : Entity
{
    public Guid ProgramId { get; set; }
    public Guid? AccountId { get; set; }
    public TransactionType Type { get; protected set; }
    public decimal Miles { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
