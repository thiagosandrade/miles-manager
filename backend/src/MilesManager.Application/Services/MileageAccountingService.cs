using MilesManager.Application.Abstractions;

namespace MilesManager.Application.Services;

public sealed class MileageAccountingService : IMileageAccountingService
{
    public decimal CalculateCostPerPoint(decimal balance, decimal allocatedCost)
    {
        return balance <= 0m ? 0m : allocatedCost / balance;
    }

    public decimal CalculateProportionalCost(decimal points, decimal balance, decimal allocatedCost)
    {
        if (points <= 0m || balance <= 0m || allocatedCost <= 0m)
        {
            return 0m;
        }

        return points * CalculateCostPerPoint(balance, allocatedCost);
    }

    public decimal CalculateBaseDestinationPoints(decimal sourcePoints, decimal fromAmount, decimal toAmount)
    {
        if (sourcePoints <= 0m || fromAmount <= 0m || toAmount <= 0m)
        {
            return 0m;
        }

        return sourcePoints / fromAmount * toAmount;
    }

    public decimal CalculateBonusPoints(decimal baseDestinationPoints, decimal bonusPercentage)
    {
        if (baseDestinationPoints <= 0m || bonusPercentage <= 0m)
        {
            return 0m;
        }

        return baseDestinationPoints * bonusPercentage / 100m;
    }

    public decimal CalculateTotalDestinationPoints(decimal baseDestinationPoints, decimal bonusPoints)
    {
        return Math.Round(Math.Max(0m, baseDestinationPoints) + Math.Max(0m, bonusPoints), 0, MidpointRounding.AwayFromZero);
    }
}
