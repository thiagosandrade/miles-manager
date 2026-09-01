using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Application.Features.Users.DTOs;
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
            .UseInMemoryDatabase($"GetUserByIdHandlerTests_{Guid.NewGuid()}").Options;

        AppUser created;
        await using (var context = new MilesManagerDbContext(options))
        {
            created = new AppUser { Email = "john@test.com", FullName = "John" };
            context.Users.Add(created);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        await using (var context = new MilesManagerDbContext(options))
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AppUser, UserDetailsDTO>();
                cfg.CreateMap<UserRole, UserRoleDTO>();
                cfg.CreateMap<Role, RoleDTO>();
            });
            var handler = new GetUserByIdHandler(new Infrastructure.Persistence.Repositories.UserRepository(context), config.CreateMapper());
            var result = await handler.Handle(new GetUserByIdQuery(created.Id), TestContext.Current.CancellationToken);
            result.Should().NotBeNull();
            result!.Email.Should().Be("john@test.com"); result.FullName.Should().Be("John");
        }
    }
}
