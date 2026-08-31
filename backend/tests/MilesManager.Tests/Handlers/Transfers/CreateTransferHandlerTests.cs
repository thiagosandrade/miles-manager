using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.Commands.CreateTransfer;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class CreateTransferHandlerTests
{
    [Fact]
    public async Task Handle_WithFactorAndBonus_MovesProportionalCostAndCalculatesReceivedMiles()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateTransfer_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Livelo", ProgramCode = "LIVELO" },
            new LoyaltyProgram { Id = destinationId, Name = "TAP", ProgramCode = "TAP" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = originId,
            Balance = 10_000m,
            AllocatedCost = 300m
        });
        context.TransferRoutes.Add(new ProgramFactor
        {
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 2m,
            ToAmount = 1m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            MilesSent = 4_000m,
            BonusPercentage = 100m,
            TransferDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balances = await context.ProgramBalances.ToListAsync(TestContext.Current.CancellationToken);
        balances.Single(x => x.ProgramId == originId).Balance.Should().Be(6_000m);
        balances.Single(x => x.ProgramId == originId).AllocatedCost.Should().Be(180m);
        balances.Single(x => x.ProgramId == destinationId).Balance.Should().Be(4_000m);
        balances.Single(x => x.ProgramId == destinationId).AllocatedCost.Should().Be(120m);

        var transfer = await context.MileageTransfers.SingleAsync(TestContext.Current.CancellationToken);
        transfer.MilesReceived.Should().Be(4_000m);
        transfer.TransferredCost.Should().Be(120m);
    }

    [Fact]
    public async Task Handle_WithoutFactor_FallsBackToOneToOne()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateTransferFallback_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "A", ProgramCode = "A" },
            new LoyaltyProgram { Id = destinationId, Name = "B", ProgramCode = "B" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = originId,
            Balance = 10_000m,
            AllocatedCost = 300m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            MilesSent = 4_000m,
            BonusPercentage = 0m,
            TransferDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var destination = await context.ProgramBalances.SingleAsync(
            x => x.ProgramId == destinationId,
            TestContext.Current.CancellationToken);
        destination.Balance.Should().Be(4_000m);
        destination.AllocatedCost.Should().Be(120m);
    }

    [Fact]
    public async Task Handle_SameProgram_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateTransferSameProgram_{Guid.NewGuid()}").Options;
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Program", ProgramCode = "P" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = Guid.NewGuid(),
            OriginProgramId = programId,
            DestinationProgramId = programId,
            MilesSent = 1_000m,
            TransferDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }
}
