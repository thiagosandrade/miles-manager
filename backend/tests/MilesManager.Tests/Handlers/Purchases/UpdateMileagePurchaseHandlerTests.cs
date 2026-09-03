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
    private static (Guid UserId, Guid ProgramId, Guid PurchaseId) CreateIds() => (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private static MileagePurchase CreatePurchase(Guid userId, Guid programId, Guid purchaseId, PurchaseStatus status, decimal miles = 500m, decimal amount = 50m) =>
        new() { Id = purchaseId, UserId = userId, ProgramId = programId, AmountPaid = amount, MilesPurchased = miles, PurchaseDate = new DateOnly(2026, 9, 1), PurchaseStatus = status };

    private static async Task SeedAsync(MilesManagerDbContext context, Guid userId, Guid programId, MileagePurchase purchase, decimal balance = 1_000m, decimal cost = 100m)
    {
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO" });
        context.ProgramBalances.Add(new ProgramBalance { UserId = userId, ProgramId = programId, Balance = balance, AllocatedCost = cost });
        context.MileagePurchases.Add(purchase);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static UpdateMileagePurchaseCommand Command(Guid userId, Guid programId, Guid purchaseId, PurchaseStatus status, decimal miles = 500m, decimal amount = 50m) =>
        new() { Id = purchaseId, UserId = userId, ProgramId = programId, AmountPaid = amount, MilesPurchased = miles, PurchaseDate = new DateOnly(2026, 9, 3), PurchaseStatus = status };

    [Fact]
    public async Task Handle_PendingToPending_DoesNotChangeBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdatePendingPending_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Pending), 1_000m, 100m);
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Pending, 600m, 60m), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_PendingToCompleted_AddsMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdatePendingToCompleted_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Pending));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Confirmed), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_500m); balance.AllocatedCost.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_PendingToCanceled_DoesNotChangeBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdatePendingToCanceled_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Pending));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Canceled), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
        (await context.MileagePurchases.SingleAsync(TestContext.Current.CancellationToken)).PurchaseStatus.Should().Be(PurchaseStatus.Canceled);
    }

    [Fact]
    public async Task Handle_CompletedToCompleted_AdjustsMilesAndCostByDelta()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCompletedDelta_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Confirmed));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Confirmed, 800m, 80m), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_300m); balance.AllocatedCost.Should().Be(130m);
    }

    [Fact]
    public async Task Handle_CompletedToPending_RemovesMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCompletedToPending_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Confirmed), 1_500m, 150m);
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Pending), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_CompletedToCanceled_RemovesMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCompletedToCanceled_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Confirmed), 1_500m, 150m);
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Canceled), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
        (await context.MileagePurchases.SingleAsync(TestContext.Current.CancellationToken)).PurchaseStatus.Should().Be(PurchaseStatus.Canceled);
    }

    [Fact]
    public async Task Handle_CanceledToCanceled_DoesNotChangeBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCanceledCanceled_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Canceled));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Canceled, 600m, 60m), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_CanceledToPending_DoesNotChangeBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCanceledPending_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Canceled));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Pending, 600m, 60m), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m); balance.AllocatedCost.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_CanceledToCompleted_AddsMilesAndCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateCanceledCompleted_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        await SeedAsync(context, userId, programId, CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Canceled));
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Confirmed, 600m, 60m), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_600m); balance.AllocatedCost.Should().Be(160m);
    }

    [Fact]
    public async Task Handle_WhenProgramBalanceMissingForCompletedTransition_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateMissingBalance_{Guid.NewGuid()}").Options;
        var (userId, programId, purchaseId) = CreateIds();
        await using var context = new MilesManagerDbContext(options);
        context.MileagePurchases.Add(CreatePurchase(userId, programId, purchaseId, PurchaseStatus.Pending));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(userId, programId, purchaseId, PurchaseStatus.Confirmed), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }

    [Fact]
    public async Task Handle_NonexistentPurchase_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"UpdateMissingPurchase_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var result = await new UpdateMileagePurchaseHandler(context).Handle(Command(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PurchaseStatus.Pending), TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
