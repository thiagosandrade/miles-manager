namespace MilesManager.Application.Features.Calculator.DTOs
{
    public sealed class FlightComparisonDto
    {
        public Guid ProgramId { get; init; }
        public string ProgramName { get; init; } = string.Empty;
        public string CurrencyCode { get; init; } = string.Empty;

        public decimal MilesRequired { get; init; }
        public decimal Taxes { get; init; }
        public decimal CashPrice { get; init; }

        public decimal ReferencePricePerThousand { get; init; }

        public decimal MilesValue { get; init; }
        public decimal TotalMilesCost { get; init; }

        public decimal Savings { get; init; }

        public decimal EffectiveValuePerThousand { get; init; }

        public bool IsMilesBetter { get; init; }
    }
}
