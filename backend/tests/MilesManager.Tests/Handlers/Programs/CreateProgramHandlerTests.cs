using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.Commands.CreateProgram;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Programs;

public class CreateProgramHandlerTests
{
    [Fact]
    public async Task Handle_NewProgram_ReturnsSuccessAndCreatesProgram()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateProgram_{Guid.NewGuid()}")
            .Options;

        await using var context = new MilesManagerDbContext(options);
        var handler = new CreateProgramHandler(context);

        var result = await handler.Handle(
            new CreateProgramCommand { ProgramCode = "TST", Name = "Test Program", ReferencePricePerThousand = 20m },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var created = await context.Programs.SingleAsync(p => p.ProgramCode == "TST", TestContext.Current.CancellationToken);
        created.Name.Should().Be("Test Program");
        (await context.ReferenceMileagePrices.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
    }

    [Fact]
    public async Task Handle_DuplicateProgramCode_ReturnsConflict()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateProgramConflict_{Guid.NewGuid()}")
            .Options;

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new MilesManager.Domain.Programs.LoyaltyProgram
        {
            ProgramCode = "TST",
            Name = "Existing"
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateProgramHandler(context);
        var result = await handler.Handle(
            new CreateProgramCommand { ProgramCode = "TST", Name = "Duplicate", ReferencePricePerThousand = 20m },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Conflict);
        (await context.Programs.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
    }
}