using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramFactors;

public class UpdateProgramFactorHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_UpdatesFactor()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgramFactor_{Guid.NewGuid()}").Options;
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();
        var factorId = Guid.NewGuid();

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

        var handler = new UpdateProgramFactorHandler(context);
        var result = await handler.Handle(new UpdateProgramFactorCommand
        {
            Id = factorId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 3m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var factor = await context.TransferRoutes.SingleAsync(TestContext.Current.CancellationToken);
        factor.FromAmount.Should().Be(3m);
        factor.ToAmount.Should().Be(1m);
    }

    [Fact]
    public async Task Handle_WhenFactorDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgramFactorNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);

        var handler = new UpdateProgramFactorHandler(context);
        var result = await handler.Handle(new UpdateProgramFactorCommand
        {
            Id = Guid.NewGuid(),
            OriginProgramId = Guid.NewGuid(),
            DestinationProgramId = Guid.NewGuid(),
            FromAmount = 2m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenNewProgramPairAlreadyExists_ReturnsConflict()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"UpdateProgramFactorConflict_{Guid.NewGuid()}").Options;
        var originA = Guid.NewGuid();
        var destinationA = Guid.NewGuid();
        var originB = Guid.NewGuid();
        var destinationB = Guid.NewGuid();
        var factorId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originA, Name = "A", ProgramCode = "A" },
            new LoyaltyProgram { Id = destinationA, Name = "B", ProgramCode = "B" },
            new LoyaltyProgram { Id = originB, Name = "C", ProgramCode = "C" },
            new LoyaltyProgram { Id = destinationB, Name = "D", ProgramCode = "D" });
        context.TransferRoutes.AddRange(
            new ProgramFactor { Id = factorId, OriginProgramId = originA, DestinationProgramId = destinationA, FromAmount = 2m, ToAmount = 1m },
            new ProgramFactor { OriginProgramId = originB, DestinationProgramId = destinationB, FromAmount = 2m, ToAmount = 1m });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new UpdateProgramFactorHandler(context);
        var result = await handler.Handle(new UpdateProgramFactorCommand
        {
            Id = factorId,
            OriginProgramId = originB,
            DestinationProgramId = destinationB,
            FromAmount = 3m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Conflict);
    }
}