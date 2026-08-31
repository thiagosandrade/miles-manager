using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;
using MilesManager.Application.Services;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class UpdateMileageTransferHandlerTests
{
    [Fact]
    public async Task Handle_WhenTransferExists_ReversesOldAccountingAndAppliesNewTransfer()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateTransfer_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var originId = Guid.NewGuid(); var destinationId = Guid.NewGuid(); var transferId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.ProgramBalances.AddRange(
            new ProgramBalance { UserId = userId, ProgramId = originId, Balance = 6000, AllocatedCost = 180 },
            new ProgramBalance { UserId = userId, ProgramId = destinationId, Balance = 4000, AllocatedCost = 120 });
        context.TransferRoutes.Add(new ProgramFactor { OriginProgramId = originId, DestinationProgramId = destinationId, FromAmount = 2, ToAmount = 1 });
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = userId, OriginProgramId = originId, DestinationProgramId = destinationId, MilesSent = 4000, MilesReceived = 4000, TransferredCost = 120, BonusPercentage = 100, TransferDate = new DateOnly(2026, 8, 30) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateMileageTransferHandler(context, new MileageAccountingService());
        var result = await handler.Handle(new UpdateMileageTransferCommand
        {
            Id = transferId, UserId = userId, OriginProgramId = originId, DestinationProgramId = destinationId,
            MilesSent = 2000, BonusPercentage = 0, TransferDate = new DateOnly(2026, 8, 31)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balances = await context.ProgramBalances.ToListAsync(TestContext.Current.CancellationToken);
        balances.Single(x => x.ProgramId == originId).Balance.Should().Be(8000);
        balances.Single(x => x.ProgramId == originId).AllocatedCost.Should().Be(240);
        balances.Single(x => x.ProgramId == destinationId).Balance.Should().Be(1000);
        balances.Single(x => x.ProgramId == destinationId).AllocatedCost.Should().Be(60);
        var transfer = await context.MileageTransfers.SingleAsync(TestContext.Current.CancellationToken);
        transfer.MilesSent.Should().Be(2000);
        transfer.MilesReceived.Should().Be(1000);
        transfer.TransferredCost.Should().Be(60);
        transfer.BonusPercentage.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenTransferDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateTransferNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var handler = new UpdateMileageTransferHandler(context, new MileageAccountingService());
        var result = await handler.Handle(new UpdateMileageTransferCommand { Id = Guid.NewGuid() }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenChangingPrograms_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateTransferPrograms_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var originId = Guid.NewGuid(); var destinationId = Guid.NewGuid(); var otherId = Guid.NewGuid(); var transferId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = userId, OriginProgramId = originId, DestinationProgramId = destinationId, MilesSent = 1000, MilesReceived = 1000, TransferredCost = 30 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new UpdateMileageTransferHandler(context, new MileageAccountingService());
        var result = await handler.Handle(new UpdateMileageTransferCommand { Id = transferId, UserId = userId, OriginProgramId = otherId, DestinationProgramId = destinationId, MilesSent = 1000 }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }

    [Fact]
    public async Task Handle_WhenMilesSentIsNotPositive_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateTransferMiles_{Guid.NewGuid()}").Options;
        var id = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.MileageTransfers.Add(new MileageTransfer { Id = id, OriginProgramId = Guid.NewGuid(), DestinationProgramId = Guid.NewGuid(), MilesSent = 1000 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new UpdateMileageTransferHandler(context, new MileageAccountingService());
        var result = await handler.Handle(new UpdateMileageTransferCommand { Id = id, MilesSent = 0 }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }
}