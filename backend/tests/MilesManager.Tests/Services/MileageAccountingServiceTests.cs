using MilesManager.Application.Abstractions;
using MilesManager.Application.Services;
using Xunit;

namespace MilesManager.Tests.Services;

public class MileageAccountingServiceTests
{
    private readonly IMileageAccountingService _service = new MileageAccountingService();

    [Fact]
    public void CalculateCostPerPoint_ShouldReturnAllocatedCostPerPoint()
    {
        var result = _service.CalculateCostPerPoint(10_000m, 300m);

        Assert.Equal(0.03m, result);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldAllocateCostBasedOnTransferredPoints()
    {
        var result = _service.CalculateProportionalCost(4_000m, 10_000m, 300m);

        Assert.Equal(120m, result);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldApplyTransferFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(4_000m, 2m, 1m);

        Assert.Equal(2_000m, result);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldUseOneToOneForInvalidFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(4_000m, 0m, 1m);

        Assert.Equal(4_000m, result);
    }

    [Fact]
    public void CalculateBonusPoints_ShouldApplyPercentageToBasePoints()
    {
        var result = _service.CalculateBonusPoints(2_000m, 100m);

        Assert.Equal(2_000m, result);
    }

    [Fact]
    public void CalculateTotalDestinationPoints_ShouldIncludeBonus()
    {
        var result = _service.CalculateTotalDestinationPoints(2_000m, 2_000m);

        Assert.Equal(4_000m, result);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldReturnZeroForZeroBalance()
    {
        var result = _service.CalculateProportionalCost(1_000m, 0m, 100m);

        Assert.Equal(0m, result);
    }
}
