using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Application.Features.Users.Queries.GetUserById;
using MilesManager.Domain.Users;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Users;

public class GetUserByIdHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserDetails_WhenUserExists()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "GetUserByIdHandlerTests_Db")
            .Options;

        Domain.Users.AppUser created;
        await using (var context = new MilesManagerDbContext(options))
        {
            created = new Domain.Users.AppUser { Email = "john@test.com", FullName = "John" };
            context.Users.Add(created);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var context = new MilesManagerDbContext(options))
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRole,UserRoleDTO>();
                cfg.CreateMap<Role,RoleDTO>();
            });
            var mapper = config.CreateMapper();

            var repo = new Infrastructure.Persistence.Repositories.UserRepository(context);
            var handler = new GetUserByIdHandler(repo, mapper);

            var result = await handler.Handle(new GetUserByIdQuery(created.Id), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Email.Should().Be("john@test.com");
            result.FullName.Should().Be("John");
        }
    }
}
