namespace MilesManager.Application.Abstractions;

public interface IMileageAccountingService
{
    decimal CalculateCostPerPoint(decimal balance, decimal allocatedCost);

    decimal CalculateProportionalCost(decimal points, decimal balance, decimal allocatedCost);

    decimal CalculateBaseDestinationPoints(decimal sourcePoints, decimal fromAmount, decimal toAmount);

    decimal CalculateBonusPoints(decimal baseDestinationPoints, decimal bonusPercentage);

    decimal CalculateTotalDestinationPoints(decimal baseDestinationPoints, decimal bonusPoints);
}
