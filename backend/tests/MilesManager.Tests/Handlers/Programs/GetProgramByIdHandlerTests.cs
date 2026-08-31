using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.Queries.GetProgramById;
using MilesManager.Application.Features.Programs.DTOs;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Programs;

public class GetProgramByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenProgramExists_ReturnsMappedProgram()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetProgramById_{Guid.NewGuid()}")
            .Options;
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram
        {
            Id = programId,
            ProgramCode = "TST",
            Name = "Test Program",
            CurrencyCode = "BRL",
            IsActive = true
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var mapper = new MapperConfiguration(cfg =>
            cfg.CreateMap<LoyaltyProgram, ProgramDto>()).CreateMapper();
        var handler = new GetProgramByIdHandler(context, mapper);

        var result = await handler.Handle(
            new GetProgramByIdQuery { Id = programId },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(programId);
        result.Value.ProgramCode.Should().Be("TST");
        result.Value.Name.Should().Be("Test Program");
        result.Value.CurrencyCode.Should().Be("BRL");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenProgramDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetProgramByIdNotFound_{Guid.NewGuid()}")
            .Options;
        await using var context = new MilesManagerDbContext(options);

        var mapper = new MapperConfiguration(cfg =>
            cfg.CreateMap<LoyaltyProgram, ProgramDto>()).CreateMapper();
        var handler = new GetProgramByIdHandler(context, mapper);

        var result = await handler.Handle(
            new GetProgramByIdQuery { Id = Guid.NewGuid() },
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
