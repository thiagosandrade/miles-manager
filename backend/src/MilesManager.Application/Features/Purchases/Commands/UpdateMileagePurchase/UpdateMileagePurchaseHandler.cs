using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;

public sealed class UpdateMileagePurchaseHandler(IApplicationDbContext context) : IRequestHandler<UpdateMileagePurchaseCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(UpdateMileagePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _context.MileagePurchases.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (purchase is null)
        {
            return Result<Guid>.NotFound();
        }

        if (request.AmountPaid <= 0)
        {
            return Result<Guid>.ValidationFailed("AmountPaid must be positive.");
        }

        if (request.MilesPurchased <= 0)
        {
            return Result<Guid>.ValidationFailed("MilesPurchased must be positive.");
        }

        var milesDelta = request.MilesPurchased - purchase.MilesPurchased;

        purchase.Partner = request.Partner;
        purchase.Description = request.Description;
        purchase.AmountPaid = request.AmountPaid;
        purchase.MilesPurchased = request.MilesPurchased;
        purchase.PurchaseDate = request.PurchaseDate;

        if (milesDelta != 0)
        {
            var balance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == purchase.UserId && pb.ProgramId == purchase.ProgramId, cancellationToken);
            if (balance is null)
            {
                return Result<Guid>.ValidationFailed("Program balance not found for this user and program.");
            }

            balance.Balance += milesDelta;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(purchase.Id);
    }
}
