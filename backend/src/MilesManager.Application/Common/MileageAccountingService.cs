using MilesManager.Application.Abstractions;

namespace MilesManager.Application.Common;

public sealed class MileageAccountingService : IMileageAccountingService
{
    public decimal CalculateCostPerPoint(decimal balance, decimal allocatedCost)
    {
        if (balance <= 0)
        {
            return 0m;
        }

        return allocatedCost / balance;
    }

    public decimal CalculateProportionalCost(decimal points, decimal balance, decimal allocatedCost)
    {
        if (points <= 0 || balance <= 0 || allocatedCost <= 0)
        {
            return 0m;
        }

        return Math.Round(
            points * CalculateCostPerPoint(balance, allocatedCost),
            2,
            MidpointRounding.AwayFromZero);
    }

    public decimal CalculateBaseDestinationPoints(decimal sourcePoints, decimal fromAmount, decimal toAmount)
    {
        if (sourcePoints <= 0 || fromAmount <= 0 || toAmount <= 0)
        {
            return 0m;
        }

        return sourcePoints / fromAmount * toAmount;
    }

    public decimal CalculateBonusPoints(decimal baseDestinationPoints, decimal bonusPercentage)
    {
        if (baseDestinationPoints <= 0 || bonusPercentage <= 0)
        {
            return 0m;
        }

        return baseDestinationPoints * bonusPercentage / 100m;
    }

    public decimal CalculateTotalDestinationPoints(decimal baseDestinationPoints, decimal bonusPoints)
    {
        return baseDestinationPoints + bonusPoints;
    }
}
