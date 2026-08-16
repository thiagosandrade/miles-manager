using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Application.Features.Users.Queries.GetUsers;
using MilesManager.Domain.Users;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Users;

public class GetUsersHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllUsersOrderedByEmail()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "GetUsersHandlerTests_Db")
            .Options;

        await using (var context = new MilesManagerDbContext(options))
        {
            context.Users.Add(new Domain.Users.AppUser { Email = "b@test.com", FullName = "B" });
            context.Users.Add(new Domain.Users.AppUser { Email = "a@test.com", FullName = "A" });
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var context = new MilesManagerDbContext(options))
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRole, UserRoleDTO>();
                cfg.CreateMap<Role, RoleDTO>();
            });
            var mapper = config.CreateMapper();

            var repo = new Infrastructure.Persistence.Repositories.UserRepository(context);
            var handler = new GetUsersHandler(repo, mapper);

            var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Email.Should().Be("a@test.com");
            result[1].Email.Should().Be("b@test.com");
        }
    }
}
