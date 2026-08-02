using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Transactions;

public sealed class TransferTransaction : Transaction
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public string? TransferReason { get; set; }
    public TransferTransaction()
    {
        Type = TransactionType.Transfer;
    }
}
