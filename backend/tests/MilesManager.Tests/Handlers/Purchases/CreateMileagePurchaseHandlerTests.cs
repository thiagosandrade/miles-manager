using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;
using MilesManager.Infrastructure.Persistence;
using MilesManager.Domain.Programs;
using Xunit;

namespace MilesManager.Tests.Handlers.Purchases;

public class CreateMileagePurchaseHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPurchaseAndInitializesProgramBalanceWithCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreatePurchase_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        await context.SaveChangesAsync();

        var handler = new CreateMileagePurchaseHandler(context);
        var result = await handler.Handle(new CreateMileagePurchaseCommand(
            userId, programId, "Partner", "Purchase", 300m, 10_000m,
            new DateOnly(2026, 8, 30)), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync();
        balance.Balance.Should().Be(10_000m);
        balance.AllocatedCost.Should().Be(300m);
        (await context.MileagePurchases.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_ExistingBalance_AccumulatesMilesAndAllocatedCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreatePurchaseExisting_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 5_000m,
            AllocatedCost = 150m
        });
        await context.SaveChangesAsync();

        var handler = new CreateMileagePurchaseHandler(context);
        var result = await handler.Handle(new CreateMileagePurchaseCommand(
            userId, programId, null, null, 150m, 5_000m,
            new DateOnly(2026, 8, 30)), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync();
        balance.Balance.Should().Be(10_000m);
        balance.AllocatedCost.Should().Be(300m);
    }

    [Fact]
    public async Task Handle_UnknownProgram_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreatePurchaseNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var handler = new CreateMileagePurchaseHandler(context);

        var result = await handler.Handle(new CreateMileagePurchaseCommand(
            Guid.NewGuid(), Guid.NewGuid(), null, null, 100m, 1_000m,
            new DateOnly(2026, 8, 30)), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
