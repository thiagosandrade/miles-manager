using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Transfers.Commands.CreateTransfer;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class OverrideTransferFactorTests
{
    [Fact]
    public async Task Create_UsesProvidedFactorInsteadOfConfiguredFactor()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"OverrideFactorCreate_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Origin", ProgramCode = "ORG" },
            new LoyaltyProgram { Id = destinationId, Name = "Destination", ProgramCode = "DST" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = originId,
            Balance = 10_000m,
            AllocatedCost = 300m
        });
        context.TransferRoutes.Add(new ProgramFactor
        {
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            FromAmount = 2m,
            ToAmount = 1m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            MilesSent = 4_000m,
            BonusPercentage = 0m,
            FactorFrom = 1.5m,
            FactorTo = 2m,
            TransferDate = new DateOnly(2026, 9, 1)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        var destination = await context.ProgramBalances.SingleAsync(
            x => x.ProgramId == destinationId,
            TestContext.Current.CancellationToken);
        destination.Balance.Should().Be(5_333m);
    }

    [Fact]
    public async Task Create_WithOnlyOneFactorValue_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"OverrideFactorPartial_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Origin", ProgramCode = "ORG" },
            new LoyaltyProgram { Id = destinationId, Name = "Destination", ProgramCode = "DST" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = originId,
            Balance = 10_000m,
            AllocatedCost = 300m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            MilesSent = 1_000m,
            FactorFrom = 1.5m,
            TransferDate = new DateOnly(2026, 9, 1)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }

    [Fact]
    public async Task Create_WithNonPositiveFactor_ReturnsValidationFailure()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"OverrideFactorInvalid_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(
            new LoyaltyProgram { Id = originId, Name = "Origin", ProgramCode = "ORG" },
            new LoyaltyProgram { Id = destinationId, Name = "Destination", ProgramCode = "DST" });
        context.ProgramBalances.Add(new ProgramBalance
        {
            UserId = userId,
            ProgramId = originId,
            Balance = 10_000m,
            AllocatedCost = 300m
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new CreateTransferHandler(context, new Application.Services.MileageAccountingService());
        var result = await handler.Handle(new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = originId,
            DestinationProgramId = destinationId,
            MilesSent = 1_000m,
            FactorFrom = 0m,
            FactorTo = 1m,
            TransferDate = new DateOnly(2026, 9, 1)
        }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.ValidationFailed);
    }
}
