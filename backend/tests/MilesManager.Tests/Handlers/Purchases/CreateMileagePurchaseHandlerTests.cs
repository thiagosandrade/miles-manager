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
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateMileagePurchaseHandler(context);
        var result = await handler.Handle(new CreateMileagePurchaseCommand
        {
            UserId = userId,
            ProgramId = programId,
            Partner = "Partner",
            Description = "Purchase",
            AmountPaid = 300m,
            MilesPurchased = 10_000m,
            PurchaseDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(10_000m);
        balance.AllocatedCost.Should().Be(300m);
        (await context.MileagePurchases.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
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
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateMileagePurchaseHandler(context);
        var result = await handler.Handle(new CreateMileagePurchaseCommand
        {
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 150m,
            MilesPurchased = 5_000m,
            PurchaseDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
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

        var result = await handler.Handle(new CreateMileagePurchaseCommand
        {
            UserId = Guid.NewGuid(),
            ProgramId = Guid.NewGuid(),
            AmountPaid = 100m,
            MilesPurchased = 1_000m,
            PurchaseDate = new DateOnly(2026, 8, 30)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
