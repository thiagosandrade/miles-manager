using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramBalances;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramBalances;

public class GetUserProgramPurchasesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserProgramBalancesWithLatestReferencePrice()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetUserProgramBalances_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var otherUserId = Guid.NewGuid(); var programOneId = Guid.NewGuid(); var programTwoId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = programOneId, Name = "Program One", ProgramCode = "P1" },
            new LoyaltyProgram { Id = programTwoId, Name = "Program Two", ProgramCode = "P2" });
        context.ProgramBalances.AddRange(
            new ProgramBalance { UserId = userId, ProgramId = programOneId, Balance = 1500m, AllocatedCost = 20m },
            new ProgramBalance { UserId = userId, ProgramId = programTwoId, Balance = 2000m, AllocatedCost = 0m },
            new ProgramBalance { UserId = otherUserId, ProgramId = programOneId, Balance = 9999m, AllocatedCost = 999m });
        context.ReferenceMileagePrices.AddRange(
            new ReferenceMileagePrice { ProgramId = programOneId, PricePerThousand = 10m, EffectiveDate = new DateOnly(2025, 1, 1) },
            new ReferenceMileagePrice { ProgramId = programOneId, PricePerThousand = 15m, EffectiveDate = new DateOnly(2026, 1, 1) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetUserProgramBalancesHandler(context);
        var result = await handler.Handle(new GetUserProgramBalancesQuery(userId), TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().HaveCount(2);
        var programOne = result.Value!.Single(x => x.ProgramId == programOneId);
        programOne.Balance.Should().Be(1500m);
        programOne.CurrentValue.Should().Be(22.5m);
        var programTwo = result.Value.Single(x => x.ProgramId == programTwoId);
        programTwo.Balance.Should().Be(2000m);
        programTwo.CurrentValue.Should().Be(0m);
    }
}
