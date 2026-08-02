using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Transactions;

public sealed class ExpirationTransaction : Transaction
{
    public DateOnly ExpiredOn { get; set; }
    public string? ExpirationRule { get; set; }
    public ExpirationTransaction()
    {
        Type = TransactionType.Expiration;
    }
}
