using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Commands.UpdateFlightRedemption;

public sealed class UpdateFlightRedemptionHandler(IApplicationDbContext context) : IRequestHandler<UpdateFlightRedemptionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(UpdateFlightRedemptionCommand request, CancellationToken cancellationToken)
    {
        var redemption = await _context.FlightRedemptions.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (redemption is null)
        {
            return Result<Guid>.NotFound();
        }

        if (redemption.ProgramId != request.ProgramId)
        {
            return Result<Guid>.ValidationFailed("Changing program is not supported.");
        }

        var balance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.ProgramId, cancellationToken);
        if (balance is null)
        {
            return Result<Guid>.ValidationFailed("Program balance not found for this user and program.");
        }

        var deltaMiles = request.MilesUsed - redemption.MilesUsed;
        if (deltaMiles > 0 && balance.Balance < deltaMiles)
        {
            return Result<Guid>.ValidationFailed("Program balance does not have enough miles for the update.");
        }

        balance.Balance -= deltaMiles;

        redemption.OriginAirportCode = request.OriginAirportCode;
        redemption.DestinationAirportCode = request.DestinationAirportCode;
        redemption.FlightDate = request.FlightDate;
        redemption.CabinClass = request.CabinClass;
        redemption.Passengers = request.Passengers;
        redemption.IsRoundTrip = request.IsRoundTrip;
        redemption.MilesUsed = request.MilesUsed;
        redemption.Taxes = request.Taxes;
        redemption.CashPrice = request.CashPrice;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(redemption.Id);
    }
}
