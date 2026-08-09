using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
            .UseInMemoryDatabase(databaseName: "CreateProgramTests_NewProgram")
            .Options;

        await using var context = new MilesManagerDbContext(options);

        var handler = new CreateProgramHandler(context);

        var command = new CreateProgramCommand { Code = "TST", Name = "Test Program" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
        var created = await context.Programs.FirstOrDefaultAsync(p => p.ProgramCode == "TST", CancellationToken.None);
        created.Should().NotBeNull();
        created!.Name.Should().Be("Test Program");
    }
}
