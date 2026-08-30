using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Commands.CreateTransfer;

public sealed class CreateTransferHandler(
    IApplicationDbContext context,
    IMileageAccountingService accountingService) : IRequestHandler<CreateTransferCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMileageAccountingService _accountingService = accountingService;

    public async Task<Result<Guid>> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        var origin = await _context.Programs.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.OriginProgramId, cancellationToken);
        var destination = await _context.Programs.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.DestinationProgramId, cancellationToken);

        if (origin is null || destination is null)
        {
            return Result<Guid>.NotFound();
        }

        if (request.OriginProgramId == request.DestinationProgramId)
        {
            return Result<Guid>.ValidationFailed("Origin and destination programs must be different.");
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

        if (originBalance is null || originBalance.Balance < request.MilesSent)
        {
            return Result<Guid>.ValidationFailed("Origin program balance does not have enough miles.");
        }

        var transferredCost = _accountingService.CalculateProportionalCost(
            request.MilesSent,
            originBalance.Balance,
            originBalance.AllocatedCost);

        var factor = await _context.TransferRoutes.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OriginProgramId == request.OriginProgramId &&
                     x.DestinationProgramId == request.DestinationProgramId,
                cancellationToken);

        var fromAmount = factor?.FromAmount ?? 1m;
        var toAmount = factor?.ToAmount ?? 1m;

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

        var destinationBalance = await _context.ProgramBalances.FirstOrDefaultAsync(
            pb => pb.UserId == request.UserId && pb.ProgramId == request.DestinationProgramId,
            cancellationToken);

        if (destinationBalance is null)
        {
            destinationBalance = new Domain.Programs.ProgramBalance
            {
                UserId = request.UserId,
                ProgramId = request.DestinationProgramId,
                Balance = milesReceived,
                AllocatedCost = transferredCost
            };
            await _context.ProgramBalances.AddAsync(destinationBalance, cancellationToken);
        }
        else
        {
            destinationBalance.Balance += milesReceived;
            destinationBalance.AllocatedCost += transferredCost;
        }

        var transfer = new Domain.Transfers.MileageTransfer
        {
            UserId = request.UserId,
            OriginProgramId = request.OriginProgramId,
            DestinationProgramId = request.DestinationProgramId,
            MilesSent = request.MilesSent,
            BonusPercentage = request.BonusPercentage,
            MilesReceived = milesReceived,
            TransferredCost = transferredCost,
            TransferDate = request.TransferDate
        };

        await _context.MileageTransfers.AddAsync(transfer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transfer.Id);
    }
}
