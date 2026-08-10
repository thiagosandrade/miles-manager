#nullable enable
namespace MilesManager.Application.Features.Dashboard.DTOs;

public sealed class DashboardDto
{
    public decimal TotalMiles { get; init; }
    public decimal TotalInvestment { get; init; }
    public decimal AverageCostPerThousand { get; init; }
    public decimal CurrentMarketValue { get; init; }
    public decimal CurrentProfit { get; init; }
    public decimal CurrentLoss { get; init; }
    public IReadOnlyList<object> Programs { get; init; } = [];
    public IReadOnlyList<object> RecentPurchases { get; init; } = [];
    public IReadOnlyList<object> RecentTransfers { get; init; } = [];
    public IReadOnlyList<object> RecentRedemptions { get; init; } = [];
}
