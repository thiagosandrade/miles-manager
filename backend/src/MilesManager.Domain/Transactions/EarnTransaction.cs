using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Transactions;

public sealed class EarnTransaction : Transaction
{
    public Guid? PartnerId { get; set; }
    public string? SourceDescription { get; set; }
    public EarnTransaction()
    {
        Type = TransactionType.Earn;
    }
}
