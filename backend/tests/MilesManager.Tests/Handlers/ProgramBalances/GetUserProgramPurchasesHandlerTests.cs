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
    public async Task Handle_ReturnsUserProgramPurchasesWithLatestPriceAndFallback()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetUserProgramPurchasesTests_{Guid.NewGuid()}")
            .Options;

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var programOneId = Guid.NewGuid();
        var programTwoId = Guid.NewGuid();

        await using (var context = new MilesManagerDbContext(options))
        {
            context.Programs.AddRange(
                new LoyaltyProgram
                {
                    Id = programOneId,
                    Name = "Program One",
                    ProgramCode = "P1"
                },
                new LoyaltyProgram
                {
                    Id = programTwoId,
                    Name = "Program Two",
                    ProgramCode = "P2"
                });

            context.ReferenceMileagePrices.AddRange(
                new ReferenceMileagePrice
                {
                    ProgramId = programOneId,
                    PricePerThousand = 10m,
                    EffectiveDate = new DateOnly(2025, 1, 1)
                },
                new ReferenceMileagePrice
                {
                    ProgramId = programOneId,
                    PricePerThousand = 15m,
                    EffectiveDate = new DateOnly(2026, 1, 1)
                });

            context.MileagePurchases.AddRange(
                new MileagePurchase
                {
                    UserId = userId,
                    ProgramId = programOneId,
                    MilesPurchased = 1000m,
                    PurchaseDate = new DateOnly(2026, 1, 10)
                },
                new MileagePurchase
                {
                    UserId = userId,
                    ProgramId = programOneId,
                    MilesPurchased = 500m,
                    PurchaseDate = new DateOnly(2026, 2, 10)
                },
                new MileagePurchase
                {
                    UserId = otherUserId,
                    ProgramId = programOneId,
                    MilesPurchased = 9999m,
                    PurchaseDate = new DateOnly(2026, 3, 10)
                },
                new MileagePurchase
                {
                    UserId = otherUserId,
                    ProgramId = programTwoId,
                    MilesPurchased = 2000m,
                    PurchaseDate = new DateOnly(2026, 3, 15)
                });

            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var context = new MilesManagerDbContext(options))
        {
            var handler = new GetUserProgramBalancesHandler(context);

            var result = await handler.Handle(new GetUserProgramBalancesQuery(userId), CancellationToken.None);

            result.Status.Should().Be(ResultStatus.Success);
            result.Value.Should().NotBeNull();
            result.Value!.Should().HaveCount(2);

            var programOne = result.Value.Single(x => x.ProgramId == programOneId);
            programOne.Balance.Should().Be(1500m);
            programOne.CurrentValue.Should().Be(22.5m);

            var programTwo = result.Value.Single(x => x.ProgramId == programTwoId);
            programTwo.Balance.Should().Be(0m);
            programTwo.CurrentValue.Should().Be(0m);
        }
    }
}
