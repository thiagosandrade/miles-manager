using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Redemptions.Commands.CreateRedemption;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Redemptions;

public class CreateRedemptionHandlerTests
{
    [Fact]
    public async Task Handle_ConsumesMilesAndProportionalAllocatedCost()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateRedemption_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "TAP", ProgramCode = "TAP" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 24_000m,
            AllocatedCost = 1_120m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateRedemptionHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateRedemptionCommand
        {
            UserId = userId,
            ProgramId = programId,
            MilesUsed = 12_000m,
            FlightDate = new DateOnly(2026, 8, 30),
            Passengers = 1
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(12_000m);
        balance.AllocatedCost.Should().Be(560m);
        (await context.FlightRedemptions.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ReturnsValidationFailureAndDoesNotCreateRedemption()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateRedemptionInsufficient_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "TAP", ProgramCode = "TAP" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = programId,
            Balance = 1_000m,
            AllocatedCost = 50m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateRedemptionHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateRedemptionCommand
        {
            UserId = userId,
            ProgramId = programId,
            MilesUsed = 2_000m,
            FlightDate = new DateOnly(2026, 8, 30),
            Passengers = 1
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.ValidationFailed);
        (await context.FlightRedemptions.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
        var balance = await context.ProgramBalances.SingleAsync(TestContext.Current.CancellationToken);
        balance.Balance.Should().Be(1_000m);
        balance.AllocatedCost.Should().Be(50m);
    }
}
