using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Redemptions.Commands.DeleteFlightRedemption;

public sealed class DeleteFlightRedemptionHandler(IApplicationDbContext context) : IRequestHandler<DeleteFlightRedemptionCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(DeleteFlightRedemptionCommand request, CancellationToken cancellationToken)
    {
        var redemption = await _context.FlightRedemptions.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (redemption is null)
        {
            return Result.NotFound();
        }

        var balance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == redemption.UserId && pb.ProgramId == redemption.ProgramId, cancellationToken);
        if (balance is null)
        {
            balance = new Domain.Programs.ProgramBalance
            {
                UserId = redemption.UserId,
                ProgramId = redemption.ProgramId,
                Balance = redemption.MilesUsed
            };
            await _context.ProgramBalances.AddAsync(balance, cancellationToken);
        }
        else
        {
            balance.Balance += redemption.MilesUsed;
        }

        _context.FlightRedemptions.Remove(redemption);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
