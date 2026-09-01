using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.ProgramFactors;

public class CreateProgramFactorHandlerTests
{
    [Fact]
    public async Task Handle_WithValidPrograms_CreatesFactor()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateProgramFactor_{Guid.NewGuid()}").Options;
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Livelo", ProgramCode = "LIVELO" },
            new LoyaltyProgram { Id = destinationId, Name = "TAP", ProgramCode = "TAP" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateProgramFactorHandler(context);
        var result = await handler.Handle(new CreateProgramFactorCommand
        {
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 2m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var factor = await context.TransferRoutes.SingleAsync(TestContext.Current.CancellationToken);
        factor.OriginProgramId.Should().Be(originId);
        factor.DestinationProgramId.Should().Be(destinationId);
        factor.FromAmount.Should().Be(2m);
        factor.ToAmount.Should().Be(1m);
    }

    [Fact]
    public async Task Handle_WhenProgramDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateProgramFactorNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var handler = new CreateProgramFactorHandler(context);

        var result = await handler.Handle(new CreateProgramFactorCommand
        {
            OriginProgramId = Guid.NewGuid(),
            DestinationProgramId = Guid.NewGuid(),
            FromAmount = 2m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenFactorAlreadyExists_ReturnsConflict()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"CreateProgramFactorConflict_{Guid.NewGuid()}").Options;
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Livelo", ProgramCode = "LIVELO" },
            new LoyaltyProgram { Id = destinationId, Name = "TAP", ProgramCode = "TAP" });
        context.TransferRoutes.Add(new Domain.Transfers.ProgramFactor
        {
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 2m,
            ToAmount = 1m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateProgramFactorHandler(context);
        var result = await handler.Handle(new CreateProgramFactorCommand
        {
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 3m,
            ToAmount = 1m
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Conflict);
        (await context.TransferRoutes.CountAsync(TestContext.Current.CancellationToken)).Should().Be(1);
    }
}