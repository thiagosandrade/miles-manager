using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Commands.CreateRedemption;

public sealed class CreateRedemptionHandler(
    IApplicationDbContext context,
    IMileageAccountingService accountingService) : IRequestHandler<CreateRedemptionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMileageAccountingService _accountingService = accountingService;

    public async Task<Result<Guid>> Handle(CreateRedemptionCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.Programs
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProgramId, cancellationToken);

        if (program is null)
        {
            return Result<Guid>.NotFound();
        }

        var balance = await _context.ProgramBalances
            .FirstOrDefaultAsync(
                pb => pb.UserId == request.UserId && pb.ProgramId == request.ProgramId,
                cancellationToken);

        if (balance is null || balance.Balance < request.MilesUsed)
        {
            return Result<Guid>.ValidationFailed("Program balance does not have enough miles.");
        }

        var allocatedCost = _accountingService.CalculateProportionalCost(
            request.MilesUsed,
            balance.Balance,
            balance.AllocatedCost);

        balance.Balance -= request.MilesUsed;
        balance.AllocatedCost -= allocatedCost;

        var redemption = new Domain.Redemptions.FlightRedemption
        {
            UserId = request.UserId,
            ProgramId = request.ProgramId,
            OriginAirportCode = request.OriginAirportCode,
            DestinationAirportCode = request.DestinationAirportCode,
            FlightDate = request.FlightDate,
            CabinClass = request.CabinClass,
            Passengers = request.Passengers,
            IsRoundTrip = request.IsRoundTrip,
            MilesUsed = request.MilesUsed,
            Taxes = request.Taxes,
            CashPrice = request.CashPrice
        };

        await _context.FlightRedemptions.AddAsync(redemption, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(redemption.Id);
    }
}
