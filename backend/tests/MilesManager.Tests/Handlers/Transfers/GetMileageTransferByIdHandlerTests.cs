using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Context;
using MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;
using MilesManager.Domain.Programs;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class GetMileageTransferByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenTransferBelongsToUser_ReturnsTransfer()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetTransferById_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid(); var transferId = Guid.NewGuid(); var originId = Guid.NewGuid(); var destinationId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.Programs.AddRange(new LoyaltyProgram { Id = originId, Name = "Origin", ProgramCode = "ORG" }, new LoyaltyProgram { Id = destinationId, Name = "Destination", ProgramCode = "DST" });
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = userId, OriginProgramId = originId, DestinationProgramId = destinationId, MilesSent = 4000, MilesReceived = 4000, BonusPercentage = 100, TransferDate = new DateOnly(2026, 8, 30) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetMileageTransferByIdHandler(context, CreateMapper(), new TestCurrentUser(userId));
        var result = await handler.Handle(new GetMileageTransferByIdQuery { Id = transferId }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.Success);
        result.Value!.MilesSent.Should().Be(4000);
        result.Value.MilesReceived.Should().Be(4000);
    }

    [Fact]
    public async Task Handle_WhenTransferBelongsToAnotherUser_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetTransferByIdUnauthorized_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var transferId = Guid.NewGuid(); var originId = Guid.NewGuid(); var destinationId = Guid.NewGuid();
        context.Programs.AddRange(new LoyaltyProgram { Id = originId, Name = "Origin", ProgramCode = "ORG" }, new LoyaltyProgram { Id = destinationId, Name = "Destination", ProgramCode = "DST" });
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = Guid.NewGuid(), OriginProgramId = originId, DestinationProgramId = destinationId, MilesSent = 1000, MilesReceived = 1000 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetMileageTransferByIdHandler(context, CreateMapper(), new TestCurrentUser(Guid.NewGuid()));
        var result = await handler.Handle(new GetMileageTransferByIdQuery { Id = transferId }, TestContext.Current.CancellationToken);
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private static IMapper CreateMapper() => new MapperConfiguration(cfg => cfg.AddMaps(typeof(GetMileageTransferByIdHandler).Assembly)).CreateMapper();
    private sealed class TestCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid? UserId => userId; public string? Email => "test@example.com"; public string? Name => "Test User"; public bool IsAuthenticated => true; public bool IsInRole(string role) => false;
    }
}
