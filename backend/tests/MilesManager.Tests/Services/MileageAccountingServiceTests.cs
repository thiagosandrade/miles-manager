using FluentAssertions;
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

        result.Should().Be(0.03m);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldAllocateCostBasedOnTransferredPoints()
    {
        var result = _service.CalculateProportionalCost(4_000m, 10_000m, 300m);

        result.Should().Be(120m);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldAllocateFullCostWhenAllPointsAreTransferred()
    {
        var result = _service.CalculateProportionalCost(10_000m, 10_000m, 300m);

        result.Should().Be(300m);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldReturnZeroForZeroPoints()
    {
        var result = _service.CalculateProportionalCost(0m, 10_000m, 300m);

        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateProportionalCost_ShouldReturnZeroForZeroBalance()
    {
        var result = _service.CalculateProportionalCost(1_000m, 0m, 100m);

        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldApplyTransferFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(4_000m, 2m, 1m);

        result.Should().Be(2_000m);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldSupportThreeToOneFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(9_000m, 3m, 1m);

        result.Should().Be(3_000m);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldSupportFractionalDestinationFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(1_000m, 1m, 2m);

        result.Should().Be(2_000m);
    }

    [Fact]
    public void CalculateBaseDestinationPoints_ShouldUseOneToOneForInvalidFactor()
    {
        var result = _service.CalculateBaseDestinationPoints(4_000m, 0m, 1m);

        result.Should().Be(4_000m);
    }

    [Fact]
    public void CalculateBonusPoints_ShouldApplyPercentageToBasePoints()
    {
        var result = _service.CalculateBonusPoints(2_000m, 100m);

        result.Should().Be(2_000m);
    }

    [Fact]
    public void CalculateBonusPoints_ShouldReturnZeroForZeroBonus()
    {
        var result = _service.CalculateBonusPoints(2_000m, 0m);

        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateBonusPoints_ShouldSupportPartialBonus()
    {
        var result = _service.CalculateBonusPoints(2_000m, 50m);

        result.Should().Be(1_000m);
    }

    [Fact]
    public void CalculateTotalDestinationPoints_ShouldIncludeBonus()
    {
        var result = _service.CalculateTotalDestinationPoints(2_000m, 2_000m);

        result.Should().Be(4_000m);
    }

    [Fact]
    public void CalculateTotalDestinationPoints_ShouldReturnBasePointsWithoutBonus()
    {
        var result = _service.CalculateTotalDestinationPoints(2_000m, 0m);

        result.Should().Be(2_000m);
    }
}
