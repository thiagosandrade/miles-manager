using FluentAssertions;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Services;
using Xunit;

namespace MilesManager.Tests.Services;

public class MileageAccountingServiceTests
{
    private readonly IMileageAccountingService _service = new MileageAccountingService();

    [Fact]
    public void CalculateCostPerPoint_ShouldReturnAllocatedCostPerPoint() => _service.CalculateCostPerPoint(10_000m, 300m).Should().Be(0.03m);
    [Fact]
    public void CalculateProportionalCost_ShouldAllocateCostBasedOnTransferredPoints() => _service.CalculateProportionalCost(4_000m, 10_000m, 300m).Should().Be(120m);
    [Fact]
    public void CalculateProportionalCost_ShouldAllocateFullCostWhenAllPointsAreTransferred() => _service.CalculateProportionalCost(10_000m, 10_000m, 300m).Should().Be(300m);
    [Fact]
    public void CalculateProportionalCost_ShouldReturnZeroForZeroPoints() => _service.CalculateProportionalCost(0m, 10_000m, 300m).Should().Be(0m);
    [Fact]
    public void CalculateProportionalCost_ShouldReturnZeroForZeroBalance() => _service.CalculateProportionalCost(1_000m, 0m, 100m).Should().Be(0m);
    [Fact]
    public void CalculateBaseDestinationPoints_ShouldApplyTransferFactor() => _service.CalculateBaseDestinationPoints(4_000m, 2m, 1m).Should().Be(2_000m);
    [Fact]
    public void CalculateBaseDestinationPoints_ShouldSupportThreeToOneFactor() => _service.CalculateBaseDestinationPoints(9_000m, 3m, 1m).Should().Be(3_000m);
    [Fact]
    public void CalculateBaseDestinationPoints_ShouldSupportFractionalDestinationFactor() => _service.CalculateBaseDestinationPoints(1_000m, 1m, 2m).Should().Be(2_000m);
    [Fact]
    public void CalculateBaseDestinationPoints_ShouldReturnZeroForInvalidFactor() => _service.CalculateBaseDestinationPoints(4_000m, 0m, 1m).Should().Be(0m);
    [Fact]
    public void CalculateBonusPoints_ShouldApplyPercentageToBasePoints() => _service.CalculateBonusPoints(2_000m, 100m).Should().Be(2_000m);
    [Fact]
    public void CalculateBonusPoints_ShouldReturnZeroForZeroBonus() => _service.CalculateBonusPoints(2_000m, 0m).Should().Be(0m);
    [Fact]
    public void CalculateBonusPoints_ShouldSupportPartialBonus() => _service.CalculateBonusPoints(2_000m, 50m).Should().Be(1_000m);
    [Fact]
    public void CalculateTotalDestinationPoints_ShouldIncludeBonus() => _service.CalculateTotalDestinationPoints(2_000m, 2_000m).Should().Be(4_000m);
    [Fact]
    public void CalculateTotalDestinationPoints_ShouldReturnBasePointsWithoutBonus() => _service.CalculateTotalDestinationPoints(2_000m, 0m).Should().Be(2_000m);
}