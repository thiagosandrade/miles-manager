using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using MilesManager.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Purchases.Commands.DeleteMileagePurchase;

public sealed class DeleteMileagePurchaseHandler(IApplicationDbContext context) : IRequestHandler<DeleteMileagePurchaseCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(DeleteMileagePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _context.MileagePurchases.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (purchase is null)
        {
            return Result.NotFound();
        }

        if (purchase.PurchaseStatus == PurchaseStatus.Confirmed)
        {
            var balance = await _context.ProgramBalances.FirstOrDefaultAsync(
                pb => pb.UserId == purchase.UserId && pb.ProgramId == purchase.ProgramId,
                cancellationToken);

            if (balance is null)
            {
                return Result.ValidationFailed("Program balance not found for this user and program.");
            }

            balance.Balance -= purchase.MilesPurchased;
            balance.AllocatedCost -= purchase.AmountPaid;
        }

        _context.MileagePurchases.Remove(purchase);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
