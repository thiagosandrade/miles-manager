using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Email;
using MilesManager.Application.Features.Users.Commands.RegisterUser;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Users;

public class RegisterUserHandlerTests
{
    [Fact]
    public async Task Handle_CreatesInactiveUserWithHashedPassword()
    {
        await using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new RegisterUserCommand(" NewUser@Test.com ", " New User ", "password123"),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("newuser@test.com");
        result.Value.FullName.Should().Be("New User");
        result.Value.IsActive.Should().BeFalse();

        var created = await context.Users.SingleAsync(TestContext.Current.CancellationToken);
        created.IsActive.Should().BeFalse();
        created.PasswordHash.Should().Be("hashed-password123");
    }

    [Fact]
    public async Task Handle_NewUser_HasNoLoginOrUpdateTimestamps()
    {
        await using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new RegisterUserCommand("newuser@test.com", "New User", "password123"),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Success);

        var created = await context.Users.SingleAsync(TestContext.Current.CancellationToken);
        created.LastLoginAtUtc.Should().BeNull();
        created.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ReturnsConflict_WhenEmailAlreadyExists()
    {
        await using var context = CreateContext();
        context.Users.Add(new AppUser { Email = "existing@test.com", FullName = "Existing" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = CreateHandler(context);

        var result = await handler.Handle(
            new RegisterUserCommand("EXISTING@test.com", "Another User", "password123"),
            TestContext.Current.CancellationToken);

        result.Status.Should().Be(ResultStatus.Conflict);
        context.Users.Should().HaveCount(1);
    }

    private static MilesManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"RegisterUserHandlerTests_{Guid.NewGuid()}")
            .Options;

        return new MilesManagerDbContext(options);
    }

    private static RegisterUserHandler CreateHandler(MilesManagerDbContext context)
    {
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, UserDetailsDTO>()).CreateMapper();
        var emailService = new TestEmailService();

        return new RegisterUserHandler(
            new Infrastructure.Persistence.Repositories.UserRepository(context),
            new TestPasswordHasher(),
            mapper,
            emailService,
            new EmailTemplateService());
    }

    private sealed class TestPasswordHasher : MilesManager.Application.Features.Users.Abstractions.IPasswordHasher
    {
        public string HashPassword(string password) => $"hashed-{password}";
        public bool VerifyPassword(string password, string passwordHash) => passwordHash == $"hashed-{password}";
    }

    private sealed class TestEmailService : IEmailService
    {
        public Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
