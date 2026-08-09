using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Infrastructure.Persistence;
using MilesManager.Application.Features.Users.Queries.GetUserById;
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

        MilesManager.Domain.Users.AppUser created;
        await using (var context = new MilesManagerDbContext(options))
        {
            created = new MilesManager.Domain.Users.AppUser { Email = "john@test.com", FullName = "John" };
            context.Users.Add(created);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using (var context = new MilesManagerDbContext(options))
        {
            var repo = new MilesManager.Infrastructure.Persistence.Repositories.UserRepository(context);
            var handler = new GetUserByIdHandler(repo);

            var result = await handler.Handle(new GetUserByIdQuery(created.Id), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Email.Should().Be("john@test.com");
            result.FullName.Should().Be("John");
        }
    }
}
