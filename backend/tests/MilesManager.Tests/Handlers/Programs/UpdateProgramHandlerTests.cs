using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.Commands.UpdateProgram;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Programs;

public class UpdateProgramHandlerTests
{
    [Fact]
    public async Task Handle_WhenProgramExists_UpdatesProgramAndCreatesPriceHistoryWhenPriceChanges()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgram_{Guid.NewGuid()}").Options;
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram
        {
            Id = programId, ProgramCode = "OLD", Name = "Old Name", ProgramType = ProgramType.Airline,
            CurrencyCode = "BRL", IsActive = true
        });
        context.ReferenceMileagePrices.Add(new ReferenceMileagePrice
        {
            ProgramId = programId, PricePerThousand = 20m, EffectiveDate = new DateOnly(2026, 1, 1)
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateProgramHandler(context);
        var result = await handler.Handle(new UpdateProgramCommand
        {
            Id = programId, ProgramCode = " NEW ", Name = " New Name ", ProgramType = ProgramType.Airline,
            CurrencyCode = "EUR", ReferencePricePerThousand = 30m, IsActive = false
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var program = await context.Programs.SingleAsync(TestContext.Current.CancellationToken);
        program.ProgramCode.Should().Be("NEW");
        program.Name.Should().Be("New Name");
        program.CurrencyCode.Should().Be("EUR");
        program.IsActive.Should().BeFalse();
        (await context.ReferenceMileagePrices.CountAsync(TestContext.Current.CancellationToken)).Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenProgramDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgramNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var handler = new UpdateProgramHandler(context);
        var result = await handler.Handle(new UpdateProgramCommand
        {
            Id = Guid.NewGuid(), ProgramCode = "TST", Name = "Test", ReferencePricePerThousand = 10m, CurrencyCode = "BRL"
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenReferencePriceDoesNotChange_DoesNotCreateNewPriceHistory()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgramSamePrice_{Guid.NewGuid()}").Options;
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, ProgramCode = "TST", Name = "Test", CurrencyCode = "BRL" });
        context.ReferenceMileagePrices.Add(new ReferenceMileagePrice
        {
            ProgramId = programId, PricePerThousand = 25m, EffectiveDate = new DateOnly(2026, 1, 1)
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateProgramHandler(context);
        var result = await handler.Handle(new UpdateProgramCommand
        {
            Id = programId, ProgramCode = "TST", Name = "Test", ReferencePricePerThousand = 25m, CurrencyCode = "BRL"
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        (await context.ReferenceMileagePrices.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
    }
}