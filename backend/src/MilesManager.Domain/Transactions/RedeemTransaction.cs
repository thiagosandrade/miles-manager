using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Transactions;

public sealed class RedeemTransaction : Transaction
{
    public string RewardName { get; set; } = string.Empty;
    public string? RedemptionChannel { get; set; }
    public RedeemTransaction()
    {
        Type = TransactionType.Redeem;
    }
}
