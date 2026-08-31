using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.Commands.DeleteProgram;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Programs;

public class DeleteProgramHandlerTests
{
    [Fact]
    public async Task Handle_WhenProgramExists_DeletesProgram()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteProgram_{Guid.NewGuid()}").Options;
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, ProgramCode = "TST", Name = "Test" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new DeleteProgramHandler(context);
        var result = await handler.Handle(
            new DeleteProgramCommand { Id = programId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        (await context.Programs.AnyAsync(p => p.Id == programId, TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenProgramDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteProgramNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var handler = new DeleteProgramHandler(context);
        var result = await handler.Handle(
            new DeleteProgramCommand { Id = Guid.NewGuid() },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}