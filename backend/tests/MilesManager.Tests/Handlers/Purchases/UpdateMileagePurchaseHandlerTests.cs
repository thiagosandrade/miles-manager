using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Purchases;

public class UpdateMileagePurchaseHandlerTests
{
    [Fact]
    public async Task Handle_PendingToCompleted_AddsMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdatePendingToCompleted_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 1_000m,
            AllocatedCost = 100m
        });
        context.MileagePurchases.Add(new MileagePurchase
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 1),
            PurchaseStatus = PurchaseStatus.Pending
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateMileagePurchaseHandler(context);
        var result = await handler.Handle(new UpdateMileagePurchaseCommand
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 3),
            PurchaseStatus = PurchaseStatus.Confirmed
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_500m);
        balance.AllocatedCost.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_CompletedToPending_RemovesMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateCompletedToPending_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 1_500m,
            AllocatedCost = 150m
        });
        context.MileagePurchases.Add(new MileagePurchase
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 1),
            PurchaseStatus = PurchaseStatus.Confirmed
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateMileagePurchaseHandler(context);
        var result = await handler.Handle(new UpdateMileagePurchaseCommand
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 3),
            PurchaseStatus = PurchaseStatus.Pending
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m);
        balance.AllocatedCost.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_CompletedToCanceled_RemovesMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateCompletedToCanceled_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 1_500m,
            AllocatedCost = 150m
        });
        context.MileagePurchases.Add(new MileagePurchase
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 1),
            PurchaseStatus = PurchaseStatus.Confirmed
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateMileagePurchaseHandler(context);
        var result = await handler.Handle(new UpdateMileagePurchaseCommand
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 3),
            PurchaseStatus = PurchaseStatus.Canceled
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m);
        balance.AllocatedCost.Should().Be(100m);

        var purchase = await context.MileagePurchases.SingleAsync(TestContext.Current.CancellationToken);
        purchase.PurchaseStatus.Should().Be(PurchaseStatus.Canceled);
    }

    [Fact]
    public async Task Handle_CompletedPurchaseAmountAndMilesChange_AppliesDeltaToBalanceAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateCompletedDelta_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 1_500m,
            AllocatedCost = 150m
        });
        context.MileagePurchases.Add(new MileagePurchase
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 50m,
            MilesPurchased = 500m,
            PurchaseDate = new DateOnly(2026, 9, 1),
            PurchaseStatus = PurchaseStatus.Confirmed
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateMileagePurchaseHandler(context);
        var result = await handler.Handle(new UpdateMileagePurchaseCommand
        {
            Id = purchaseId,
            UserId = userId,
            ProgramId = programId,
            AmountPaid = 80m,
            MilesPurchased = 800m,
            PurchaseDate = new DateOnly(2026, 9, 3),
            PurchaseStatus = PurchaseStatus.Confirmed
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_800m);
        balance.AllocatedCost.Should().Be(180m);
    }
}
