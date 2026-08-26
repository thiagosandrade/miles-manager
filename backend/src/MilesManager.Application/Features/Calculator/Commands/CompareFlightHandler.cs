using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Calculator.DTOs;

namespace MilesManager.Application.Features.Calculator.Commands;

public sealed class CompareFlightHandler(IApplicationDbContext context)
    : IRequestHandler<CompareFlightQuery, FlightComparisonDto>
{
    public async Task<FlightComparisonDto> Handle(CompareFlightQuery request, CancellationToken cancellationToken)
    {
        if (request.MilesRequired <= 0)
        {
            throw new ArgumentException("Miles required must be greater than zero.");
        }

        if (request.Taxes < 0)
        {
            throw new ArgumentException("Taxes cannot be negative.");
        }

        if (request.CashPrice < 0)
        {
            throw new ArgumentException("Cash price cannot be negative.");
        }

        var program = await context.Programs
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.ProgramId,
                cancellationToken);

        if (program is null)
        {
            throw new KeyNotFoundException($"Loyalty program '{request.ProgramId}' was not found.");
        }

        // Get the latest reference price for this program.
        var referencePrice = await context.ReferenceMileagePrices
            .AsNoTracking()
            .Where(x => x.ProgramId == request.ProgramId)
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => new
            {
                x.PricePerThousand,
                x.EffectiveDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (referencePrice is null)
        {
            throw new InvalidOperationException($"No reference mileage price was found for program '{program.Name}'.");
        }

        var referencePricePerThousand = referencePrice.PricePerThousand;

        // What the required miles are worth according to
        // the program's current reference price.
        var milesValue = request.MilesRequired / 1000m * referencePricePerThousand;

        // Effective cost of taking the award flight:
        // reference value of miles + taxes.
        var totalMilesCost = milesValue + request.Taxes;

        // Positive = miles option is cheaper.
        // Negative = cash option is cheaper.
        var savings = request.CashPrice - totalMilesCost;

        // Value you're effectively getting from each 1,000 miles
        // when choosing the award ticket.
        var effectiveValuePerThousand = (request.CashPrice - request.Taxes) / request.MilesRequired * 1000m;

        return new FlightComparisonDto
        {
            ProgramId = program.Id,
            ProgramName = program.Name,
            CurrencyCode = program.CurrencyCode,

            MilesRequired = request.MilesRequired,
            Taxes = request.Taxes,
            CashPrice = request.CashPrice,

            ReferencePricePerThousand = referencePricePerThousand,

            MilesValue = milesValue,
            TotalMilesCost = totalMilesCost,

            Savings = savings,

            EffectiveValuePerThousand = effectiveValuePerThousand,

            IsMilesBetter = savings > 0
        };
    }
}
