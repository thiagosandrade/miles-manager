using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;

public sealed class UpdateMileageTransferHandler(IApplicationDbContext context) : IRequestHandler<UpdateMileageTransferCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(UpdateMileageTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await _context.MileageTransfers.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (transfer is null)
        {
            return Result<Guid>.NotFound();
        }

        if (transfer.OriginProgramId != request.OriginProgramId || transfer.DestinationProgramId != request.DestinationProgramId)
        {
            return Result<Guid>.ValidationFailed("Changing origin or destination is not supported.");
        }

        var originBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.OriginProgramId, cancellationToken);
        var destinationBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == request.UserId && pb.ProgramId == request.DestinationProgramId, cancellationToken);
        if (originBalance is null || destinationBalance is null)
        {
            return Result<Guid>.ValidationFailed("Program balances not found for user.");
        }

        var deltaSent = request.MilesSent - transfer.MilesSent;
        var deltaReceived = request.MilesReceived - transfer.MilesReceived;

        if (deltaSent > 0 && originBalance.Balance < deltaSent)
        {
            return Result<Guid>.ValidationFailed("Origin program balance does not have enough miles for the update.");
        }

        originBalance.Balance -= deltaSent;
        destinationBalance.Balance += deltaReceived;

        transfer.MilesSent = request.MilesSent;
        transfer.BonusPercentage = request.BonusPercentage;
        transfer.MilesReceived = request.MilesReceived;
        transfer.TransferDate = request.TransferDate;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transfer.Id);
    }
}
