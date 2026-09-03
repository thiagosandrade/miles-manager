using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;

public sealed class CreateMileagePurchaseHandler(IApplicationDbContext context) : IRequestHandler<CreateMileagePurchaseCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateMileagePurchaseCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.Programs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProgramId, cancellationToken);
        if (program is null)
        {
            return Result<Guid>.NotFound();
        }

        if (request.AmountPaid < 0)
        {
            return Result<Guid>.ValidationFailed("AmountPaid must be positive.");
        }

        if (request.MilesPurchased <= 0)
        {
            return Result<Guid>.ValidationFailed("MilesPurchased must be positive.");
        }

        var purchase = new Domain.Programs.MileagePurchase
        {
            UserId = request.UserId,
            ProgramId = request.ProgramId,
            Partner = request.Partner,
            Description = request.Description,
            AmountPaid = request.AmountPaid,
            MilesPurchased = request.MilesPurchased,
            PurchaseDate = request.PurchaseDate,
            PurchaseStatus = request.PurchaseStatus
        };

        await _context.MileagePurchases.AddAsync(purchase, cancellationToken);

        if (request.PurchaseStatus == Domain.Common.Enums.PurchaseStatus.Confirmed)
        {
            var balance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.ProgramId, cancellationToken);
            if (balance is null)
            {
                balance = new Domain.Programs.ProgramBalance
                {
                    UserId = request.UserId,
                    ProgramId = request.ProgramId,
                    Balance = request.MilesPurchased,
                    AllocatedCost = request.AmountPaid
                };
                await _context.ProgramBalances.AddAsync(balance, cancellationToken);
            }
            else
            {
                balance.Balance += request.MilesPurchased;
                balance.AllocatedCost += request.AmountPaid;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(purchase.Id);
    }
}
