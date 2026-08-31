using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;
using MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;
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
        var userId = Guid.NewGuid();
        var transferId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = userId, MilesSent = 4000, MilesReceived = 4000, BonusPercentage = 100, TransferDate = new DateOnly(2026, 8, 30) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var currentUser = new TestCurrentUser(userId);
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(GetMileageTransferByIdHandler).Assembly)).CreateMapper();
        var handler = new GetMileageTransferByIdHandler(context, mapper, currentUser);

        var result = await handler.Handle(new GetMileageTransferByIdQuery { Id = transferId }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
        result.Value!.MilesSent.Should().Be(4000);
        result.Value.MilesReceived.Should().Be(4000);
    }

    [Fact]
    public async Task Handle_WhenTransferBelongsToAnotherUser_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetTransferByIdUnauthorized_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var transferId = Guid.NewGuid();
        context.MileageTransfers.Add(new MileageTransfer { Id = transferId, UserId = Guid.NewGuid(), MilesSent = 1000, MilesReceived = 1000 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var currentUser = new TestCurrentUser(Guid.NewGuid());
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(GetMileageTransferByIdHandler).Assembly)).CreateMapper();
        var handler = new GetMileageTransferByIdHandler(context, mapper, currentUser);

        var result = await handler.Handle(new GetMileageTransferByIdQuery { Id = transferId }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.NotFound);
    }

    private sealed class TestCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
        public bool IsAuthenticated => true;
    }
}