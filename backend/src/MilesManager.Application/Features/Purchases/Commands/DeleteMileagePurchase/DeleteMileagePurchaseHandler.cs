using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
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

        var balance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == purchase.UserId && pb.ProgramId == purchase.ProgramId, cancellationToken);
        if (balance is not null)
        {
            balance.Balance -= purchase.MilesPurchased;
        }

        _context.MileagePurchases.Remove(purchase);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
