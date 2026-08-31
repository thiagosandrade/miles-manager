using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.Queries.GetAllProgramFactors;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramFactors;

public class GetAllProgramFactorsHandlerTests
{
    [Fact]
    public async Task Handle_WithFactors_ReturnsAllFactorsOrderedByProgramNames()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetAllProgramFactors_{Guid.NewGuid()}").Options;
        var liveloId = Guid.NewGuid();
        var tapId = Guid.NewGuid();
        var azulId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = liveloId, Name = "Livelo", ProgramCode = "LIVELO" },
            new LoyaltyProgram { Id = tapId, Name = "TAP", ProgramCode = "TAP" },
            new LoyaltyProgram { Id = azulId, Name = "Azul", ProgramCode = "AZUL" });
        context.TransferRoutes.AddRange(
            new ProgramFactor { OriginProgramId = liveloId, DestinationProgramId = tapId, FromAmount = 2m, ToAmount = 1m },
            new ProgramFactor { OriginProgramId = azulId, DestinationProgramId = liveloId, FromAmount = 3m, ToAmount = 1m });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetAllProgramFactorsHandler(context);
        var result = await handler.Handle(
            new GetAllProgramFactorsQuery(),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().HaveCount(2);
        result.Value![0].OriginProgramName.Should().Be("Azul");
        result.Value[0].DestinationProgramName.Should().Be("Livelo");
        result.Value[0].FromAmount.Should().Be(3m);
        result.Value[0].ToAmount.Should().Be(1m);
        result.Value[1].OriginProgramName.Should().Be("Livelo");
        result.Value[1].DestinationProgramName.Should().Be("TAP");
    }

    [Fact]
    public async Task Handle_WithNoFactors_ReturnsEmptyCollection()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetAllProgramFactorsEmpty_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var handler = new GetAllProgramFactorsHandler(context);
        var result = await handler.Handle(
            new GetAllProgramFactorsQuery(),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }
}