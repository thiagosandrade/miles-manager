using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Purchases.DTOs;
using MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Purchases;

public class GetMileagePurchaseByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenPurchaseBelongsToCurrentUser_ReturnsPurchase()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetPurchaseById_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO", CurrencyCode = "BRL" });
        context.MileagePurchases.Add(new MileagePurchase
        {
            Id = purchaseId, UserId = userId, ProgramId = programId, Partner = "Partner", Description = "Purchase",
            AmountPaid = 300m, MilesPurchased = 10_000m, PurchaseDate = new DateOnly(2026, 8, 30)
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var mapper = CreateMapper();
        var handler = new GetMileagePurchaseByIdHandler(context, mapper, new TestCurrentUser(userId));

        var result = await handler.Handle(new GetMileagePurchaseByIdQuery { Id = purchaseId }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(purchaseId);
        result.Value.ProgramName.Should().Be("Livelo");
        result.Value.CurrencyCode.Should().Be("BRL");
        result.Value.AmountPaid.Should().Be(300m);
        result.Value.MilesPurchased.Should().Be(10_000m);
    }

    [Fact]
    public async Task Handle_WhenPurchaseBelongsToAnotherUser_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetPurchaseByIdOtherUser_{Guid.NewGuid()}").Options;
        var ownerId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var purchaseId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO", CurrencyCode = "BRL" });
        context.MileagePurchases.Add(new MileagePurchase { Id = purchaseId, UserId = ownerId, ProgramId = programId, AmountPaid = 100m, MilesPurchased = 1_000m, PurchaseDate = new DateOnly(2026, 8, 30) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetMileagePurchaseByIdHandler(context, CreateMapper(), new TestCurrentUser(Guid.NewGuid()));
        var result = await handler.Handle(new GetMileagePurchaseByIdQuery { Id = purchaseId }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenPurchaseDoesNotExist_ReturnsNotFound()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetPurchaseByIdNotFound_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var handler = new GetMileagePurchaseByIdHandler(context, CreateMapper(), new TestCurrentUser(Guid.NewGuid()));

        var result = await handler.Handle(new GetMileagePurchaseByIdQuery { Id = Guid.NewGuid() }, TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.NotFound);
    }

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<MileagePurchase, MileagePurchaseDto>()
            .ForMember(d => d.ProgramName, o => o.MapFrom(s => s.Program.Name))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.Program.CurrencyCode)));
        return config.CreateMapper();
    }

    private sealed class TestCurrentUser(Guid userId) : MilesManager.Application.Context.ICurrentUser
    {
        public Guid? UserId { get; } = userId;
        public string? Email => null;
        public string? Name => null;
        public bool IsAuthenticated => true;
        public bool IsInRole(string role) => false;
    }
}