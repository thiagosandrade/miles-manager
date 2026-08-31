using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class DeleteMileageTransferHandlerTests
{
    [Fact]
    public async Task Handle_WhenTransferExists_ReversesBalancesAndDeletesTransfer()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"DeleteTransfer_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var originId = Guid.NewGuid(); var destinationId = Guid.NewGuid(); var transferId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.ProgramBalances.AddRange(
            new ProgramBalance { UserId = userId, ProgramId = originId, Balance = 6000, AllocatedCost = 180 },
            new ProgramBalance { UserId = userId, ProgramId = destinationId, Balance = 4000, AllocatedCost = 120 });
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = userId, OriginProgramId = originId, DestinationProgramId = destinationId, MilesSent = 4000, MilesReceived = 4000, TransferredCost = 120, BonusPercentage = 100 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new DeleteMileageTransferHandler(context);
        var result = await handler.Handle(new DeleteMileageTransferCommand { Id = transferId }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balances = await context.ProgramBalances.ToListAsync(TestContext.Current.CancellationToken);
        balances.Single(x => x.ProgramId == originId).Balance.Should().Be(10000);
        balances.Single(x => x.ProgramId == originId).AllocatedCost.Should().Be(300);
        balances.Single(x => x.ProgramId == destinationId).Balance.Should().Be(0);
        balances.Single(x => x.ProgramId == destinationId).AllocatedCost.Should().Be(0);
        (await context.MileageTransfers.AnyAsync(x => x.Id == transferId, TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenTransferDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"DeleteTransferNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var handler = new DeleteMileageTransferHandler(context);
        var result = await handler.Handle(new DeleteMileageTransferCommand { Id = Guid.NewGuid() }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}