using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Programs.Queries.GetPrograms;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Programs;

public class GetProgramsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsProgramsList()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "GetProgramsTests_Db")
            .Options;

        await using (var context = new MilesManagerDbContext(options))
        {
            context.Programs.Add(new Domain.Programs.LoyaltyProgram { ProgramCode = "A", Name = "A" });
            context.Programs.Add(new Domain.Programs.LoyaltyProgram { ProgramCode = "B", Name = "B" });
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var context = new MilesManagerDbContext(options))
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Domain.Programs.LoyaltyProgram, Application.Features.Programs.DTOs.ProgramDto>();
            });
            var mapper = config.CreateMapper();

            var handler = new GetProgramsHandler(context, mapper);
            var result = await handler.Handle(new GetProgramsQuery(), CancellationToken.None);

            result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
            result.Value.Should().NotBeNull();
            result.Value!.Count.Should().Be(2);
        }
    }
}
