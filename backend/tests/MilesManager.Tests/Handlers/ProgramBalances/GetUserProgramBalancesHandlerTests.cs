using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramBalances;

public class GetUserProgramBalancesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyUserBalancesAndCalculatesPurchaseCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetUserProgramBalances_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var otherUserId = Guid.NewGuid(); var programId = Guid.NewGuid(); var otherProgramId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO", CurrencyCode = "BRL" },
            new LoyaltyProgram { Id = otherProgramId, Name = "TAP", ProgramCode = "TAP", CurrencyCode = "EUR" });
        context.ProgramBalances.AddRange(
            new ProgramBalance { UserId = userId, ProgramId = programId, Balance = 6000m },
            new ProgramBalance { UserId = otherUserId, ProgramId = otherProgramId, Balance = 5000m });
        context.ReferenceMileagePrices.Add(new ReferenceMileagePrice { ProgramId = programId, PricePerThousand = 20m, EffectiveDate = new DateOnly(2026, 1, 1) });
        context.MileagePurchases.AddRange(
            new MileagePurchase { UserId = userId, ProgramId = programId, MilesPurchased = 10000m, AmountPaid = 300m, PurchaseStatus = PurchaseStatus.Confirmed },
            new MileagePurchase { UserId = userId, ProgramId = programId, MilesPurchased = 5000m, AmountPaid = 100m, PurchaseStatus = PurchaseStatus.Confirmed },
            new MileagePurchase { UserId = otherUserId, ProgramId = programId, MilesPurchased = 99999m, AmountPaid = 1m, PurchaseStatus = PurchaseStatus.Confirmed });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new global::MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramBalances.GetUserProgramBalancesHandler(context);
        var result = await handler.Handle(
            new global::MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases.GetUserProgramBalancesQuery(userId),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().ContainSingle();
        var balance = result.Value!.Single();
        balance.ProgramId.Should().Be(programId);
        balance.ProgramName.Should().Be("Livelo");
        balance.Balance.Should().Be(6000m);
        balance.CurrentValue.Should().Be(20m / 1000m * 6000m);
        balance.CurrencyCode.Should().Be("BRL");
    }

    [Fact]
    public async Task Handle_WhenNoConfirmedPurchases_ReturnsZeroCurrentValue()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetUserProgramBalancesNoPurchases_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var programId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "TAP", ProgramCode = "TAP", CurrencyCode = "EUR" });
        context.ProgramBalances.Add(new ProgramBalance { UserId = userId, ProgramId = programId, Balance = 2000m });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new global::MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramBalances.GetUserProgramBalancesHandler(context);
        var result = await handler.Handle(
            new global::MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases.GetUserProgramBalancesQuery(userId),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().ContainSingle();
        result.Value!.Single().CurrentValue.Should().Be(0m);
    }
}