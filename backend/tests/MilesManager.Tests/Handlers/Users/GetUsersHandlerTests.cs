using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Application.Features.Users.DTOs;
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
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>().UseInMemoryDatabase($"GetUsersHandlerTests_{Guid.NewGuid()}").Options;
        await using (var context = new MilesManagerDbContext(options))
        {
            context.Users.AddRange(new Domain.Users.AppUser { Email = "b@test.com", FullName = "B" }, new Domain.Users.AppUser { Email = "a@test.com", FullName = "A" });
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        await using (var context = new MilesManagerDbContext(options))
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Domain.Users.AppUser, UserDetailsDTO>();
                cfg.CreateMap<UserRole, UserRoleDTO>();
                cfg.CreateMap<Role, RoleDTO>();
            });
            var handler = new GetUsersHandler(new Infrastructure.Persistence.Repositories.UserRepository(context), config.CreateMapper());
            var result = await handler.Handle(new GetUsersQuery(), TestContext.Current.CancellationToken);
            result.Should().HaveCount(2);
            result[0].Email.Should().Be("a@test.com"); result[1].Email.Should().Be("b@test.com");
        }
    }
}