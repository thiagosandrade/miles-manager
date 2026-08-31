using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Context;
using MilesManager.Application.Features.Transfers.Queries.GetTransfers;
using MilesManager.Domain.Transfers;
using MilesManager.Infrastructure.Persistence;
using Moq;
using Xunit;

namespace MilesManager.Tests.Handlers.Transfers;

public class GetTransfersHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyCurrentUsersTransfers()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetTransfers_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        await using var context = new MilesManagerDbContext(options);
        context.MileageTransfers.AddRange(
            new MileageTransfer { UserId = userId, MilesSent = 1000, MilesReceived = 500, BonusPercentage = 0 },
            new MileageTransfer { UserId = Guid.NewGuid(), MilesSent = 2000, MilesReceived = 1000, BonusPercentage = 0 });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var mapper = CreateMapper();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(x => x.UserId).Returns(userId);
        var handler = new GetTransfersHandler(context, mapper, currentUser.Object);

        var result = await handler.Handle(new GetTransfersQuery(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().HaveCount(1);
        result.Value![0].MilesSent.Should().Be(1000);
    }

    [Fact]
    public async Task Handle_WhenNoTransfers_ReturnsEmptyCollection()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetTransfersEmpty_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var mapper = CreateMapper();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new GetTransfersHandler(context, mapper, currentUser.Object);

        var result = await handler.Handle(new GetTransfersQuery(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().BeEmpty();
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(GetTransfersHandler).Assembly));
        return configuration.CreateMapper();
    }
}