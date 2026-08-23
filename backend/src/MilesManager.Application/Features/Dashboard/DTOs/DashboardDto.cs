#nullable enable
namespace MilesManager.Application.Features.Dashboard.DTOs;

public sealed class DashboardDto
{
    public int ProgramCount { get; init; }
    public IReadOnlyList<DashboardProgramDto> Programs { get; init; } = [];
    public IReadOnlyList<object> RecentPurchases { get; init; } = [];
    public IReadOnlyList<object> RecentTransfers { get; init; } = [];
    public IReadOnlyList<object> RecentRedemptions { get; init; } = [];
}

public sealed class DashboardProgramDto
{
    public Guid ProgramId { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public decimal Miles { get; init; }
    public decimal ActualCostPerThousand { get; init; }
    public decimal ReferencePricePerThousand { get; init; }
    public decimal UnrealizedProfitLoss { get; init; }
    public decimal UnrealizedProfitLossPercentage { get; init; }
}
