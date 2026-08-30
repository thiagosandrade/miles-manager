using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;

public sealed class UpdateMileageTransferHandler(
    IApplicationDbContext context,
    IMileageAccountingService accountingService) : IRequestHandler<UpdateMileageTransferCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMileageAccountingService _accountingService = accountingService;

    public async Task<Result<Guid>> Handle(UpdateMileageTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = await _context.MileageTransfers.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (transfer is null)
        {
            return Result<Guid>.NotFound();
        }

        if (transfer.OriginProgramId != request.OriginProgramId ||
            transfer.DestinationProgramId != request.DestinationProgramId)
        {
            return Result<Guid>.ValidationFailed("Changing origin or destination is not supported.");
        }

        if (request.MilesSent <= 0)
        {
            return Result<Guid>.ValidationFailed("MilesSent must be positive.");
        }

        if (request.BonusPercentage < 0)
        {
            return Result<Guid>.ValidationFailed("BonusPercentage cannot be negative.");
        }

        var originBalance = await _context.ProgramBalances.FirstOrDefaultAsync(
            pb => pb.UserId == request.UserId && pb.ProgramId == request.OriginProgramId,
            cancellationToken);
        var destinationBalance = await _context.ProgramBalances.FirstOrDefaultAsync(
            pb => pb.UserId == request.UserId && pb.ProgramId == request.DestinationProgramId,
            cancellationToken);

        if (originBalance is null || destinationBalance is null)
        {
            return Result<Guid>.ValidationFailed("Program balances not found for user.");
        }

        if (destinationBalance.Balance < transfer.MilesReceived ||
            destinationBalance.AllocatedCost < transfer.TransferredCost)
        {
            return Result<Guid>.ValidationFailed(
                "Destination balance does not have enough miles or allocated cost to reverse the existing transfer.");
        }

        // Restore the balances to the state before the existing transfer.
        originBalance.Balance += transfer.MilesSent;
        originBalance.AllocatedCost += transfer.TransferredCost;
        destinationBalance.Balance -= transfer.MilesReceived;
        destinationBalance.AllocatedCost -= transfer.TransferredCost;

        if (originBalance.Balance < request.MilesSent)
        {
            return Result<Guid>.ValidationFailed("Origin program balance does not have enough miles for the update.");
        }

        var factor = await _context.TransferRoutes.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OriginProgramId == request.OriginProgramId &&
                     x.DestinationProgramId == request.DestinationProgramId,
                cancellationToken);

        var fromAmount = factor?.FromAmount ?? 1m;
        var toAmount = factor?.ToAmount ?? 1m;

        var transferredCost = _accountingService.CalculateProportionalCost(
            request.MilesSent,
            originBalance.Balance,
            originBalance.AllocatedCost);
        var baseDestinationPoints = _accountingService.CalculateBaseDestinationPoints(
            request.MilesSent,
            fromAmount,
            toAmount);
        var bonusPoints = _accountingService.CalculateBonusPoints(
            baseDestinationPoints,
            request.BonusPercentage);
        var milesReceived = _accountingService.CalculateTotalDestinationPoints(
            baseDestinationPoints,
            bonusPoints);

        originBalance.Balance -= request.MilesSent;
        originBalance.AllocatedCost -= transferredCost;
        destinationBalance.Balance += milesReceived;
        destinationBalance.AllocatedCost += transferredCost;

        transfer.MilesSent = request.MilesSent;
        transfer.BonusPercentage = request.BonusPercentage;
        transfer.MilesReceived = milesReceived;
        transfer.TransferredCost = transferredCost;
        transfer.TransferDate = request.TransferDate;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transfer.Id);
    }
}
