using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Purchases.Commands.DeleteMileagePurchase;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Purchases;

public class DeleteMileagePurchaseHandlerTests
{
    [Fact]
    public async Task Handle_PendingPurchase_DeletesWithoutChangingBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeletePendingPurchase_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.ProgramBalances.Add(new ProgramBalance { UserId = userId, ProgramId = programId, Balance = 5_000m, AllocatedCost = 150m });
        context.MileagePurchases.Add(new MileagePurchase { Id = purchaseId, UserId = userId, ProgramId = programId, AmountPaid = 50m, MilesPurchased = 500m, PurchaseDate = new DateOnly(2026, 9, 3), PurchaseStatus = PurchaseStatus.Pending });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await new DeleteMileagePurchaseHandler(context).Handle(
            new DeleteMileagePurchaseCommand { Id = purchaseId, UserId = userId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(5_000m);
        balance.AllocatedCost.Should().Be(150m);
        (await context.MileagePurchases.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
    }

    [Fact]
    public async Task Handle_ConfirmedPurchase_ReversesMilesAndCostAndDeletes()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteConfirmedPurchase_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.ProgramBalances.Add(new ProgramBalance { UserId = userId, ProgramId = programId, Balance = 5_500m, AllocatedCost = 200m });
        context.MileagePurchases.Add(new MileagePurchase { Id = purchaseId, UserId = userId, ProgramId = programId, AmountPaid = 50m, MilesPurchased = 500m, PurchaseDate = new DateOnly(2026, 9, 3), PurchaseStatus = PurchaseStatus.Confirmed });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await new DeleteMileagePurchaseHandler(context).Handle(
            new DeleteMileagePurchaseCommand { Id = purchaseId, UserId = userId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(5_000m);
        balance.AllocatedCost.Should().Be(150m);
        (await context.MileagePurchases.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
    }

    [Fact]
    public async Task Handle_CanceledPurchase_DeletesWithoutChangingBalance()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteCanceledPurchase_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.ProgramBalances.Add(new ProgramBalance { UserId = userId, ProgramId = programId, Balance = 5_000m, AllocatedCost = 150m });
        context.MileagePurchases.Add(new MileagePurchase { Id = purchaseId, UserId = userId, ProgramId = programId, AmountPaid = 50m, MilesPurchased = 500m, PurchaseDate = new DateOnly(2026, 9, 3), PurchaseStatus = PurchaseStatus.Canceled });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await new DeleteMileagePurchaseHandler(context).Handle(
            new DeleteMileagePurchaseCommand { Id = purchaseId, UserId = userId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(5_000m);
        balance.AllocatedCost.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_NonexistentPurchase_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteMissingPurchase_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var result = await new DeleteMileagePurchaseHandler(context).Handle(
            new DeleteMileagePurchaseCommand { Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
