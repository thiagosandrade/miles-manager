using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramFactors;

public class DeleteProgramFactorHandlerTests
{
    [Fact]
    public async Task Handle_WhenFactorExists_DeletesFactor()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteProgramFactor_{Guid.NewGuid()}").Options;
        var factorId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Livelo", ProgramCode = "LIVELO" },
            new LoyaltyProgram { Id = destinationId, Name = "TAP", ProgramCode = "TAP" });
        context.TransferRoutes.Add(new ProgramFactor
        {
            Id = factorId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 2m,
            ToAmount = 1m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new DeleteProgramFactorHandler(context);
        var result = await handler.Handle(
            new DeleteProgramFactorCommand { Id = factorId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        (await context.TransferRoutes.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenFactorDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"DeleteProgramFactorNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var handler = new DeleteProgramFactorHandler(context);
        var result = await handler.Handle(
            new DeleteProgramFactorCommand { Id = Guid.NewGuid() },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}