using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;

public sealed class DeleteMileageTransferHandler(IApplicationDbContext context) : IRequestHandler<DeleteMileageTransferCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(DeleteMileageTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await _context.MileageTransfers.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (transfer is null)
        {
            return Result.NotFound();
        }

        var originBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == transfer.UserId && pb.ProgramId == transfer.OriginProgramId, cancellationToken);
        var destinationBalance = await _context.ProgramBalances.FirstOrDefaultAsync(pb => pb.UserId == transfer.UserId && pb.ProgramId == transfer.DestinationProgramId, cancellationToken);

        if (destinationBalance is null || destinationBalance.Balance < transfer.MilesReceived)
        {
            return Result.ValidationFailed("Destination balance does not have enough miles to reverse transfer.");
        }

        if (originBalance is null)
        {
            originBalance = new MilesManager.Domain.Programs.ProgramBalance
            {
                UserId = transfer.UserId,
                ProgramId = transfer.OriginProgramId,
                Balance = transfer.MilesSent
            };
            await _context.ProgramBalances.AddAsync(originBalance, cancellationToken);
        }
        else
        {
            originBalance.Balance += transfer.MilesSent;
        }

        destinationBalance.Balance -= transfer.MilesReceived;

        _context.MileageTransfers.Remove(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
