using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Commands.CreateTransfer;

public sealed class CreateTransferHandler(IApplicationDbContext context) : IRequestHandler<CreateTransferCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        var origin = await _context.Programs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.OriginProgramId, cancellationToken);
        var destination = await _context.Programs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.DestinationProgramId, cancellationToken);
        if (origin is null || destination is null)
        {
            return Result<Guid>.NotFound();
        }

        var originBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.OriginProgramId, cancellationToken);
        if (originBalance is null || originBalance.Balance < request.MilesSent)
        {
            return Result<Guid>.ValidationFailed("Origin program balance does not have enough miles.");
        }

        originBalance.Balance -= request.MilesSent;

        var destinationBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.DestinationProgramId, cancellationToken);
        if (destinationBalance is null)
        {
            destinationBalance = new MilesManager.Domain.Programs.ProgramBalance
            {
                UserId = request.UserId,
                ProgramId = request.DestinationProgramId,
                Balance = request.MilesReceived
            };
            await _context.ProgramBalances.AddAsync(destinationBalance, cancellationToken);
        }
        else
        {
            destinationBalance.Balance += request.MilesReceived;
        }

        var transfer = new MilesManager.Domain.Transfers.MileageTransfer
        {
            UserId = request.UserId,
            OriginProgramId = request.OriginProgramId,
            DestinationProgramId = request.DestinationProgramId,
            MilesSent = request.MilesSent,
            BonusPercentage = request.BonusPercentage,
            MilesReceived = request.MilesReceived,
            CostPerThousandOrigin = request.CostPerThousandOrigin,
            CostPerThousandDestination = request.CostPerThousandDestination,
            TransferDate = request.TransferDate
        };

        await _context.MileageTransfers.AddAsync(transfer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transfer.Id);
    }
}
