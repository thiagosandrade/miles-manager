using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Purchases.DTOs;
using MilesManager.Application.Features.Purchases.Queries.GetPurchases;
using MilesManager.Domain.Programs;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Handlers.Purchases;

public class GetPurchasesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyPurchasesForCurrentUser()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetPurchases_{Guid.NewGuid()}").Options;
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        await using var context = new MilesManagerDbContext(options);
        context.Programs.Add(new LoyaltyProgram { Id = programId, Name = "Livelo", ProgramCode = "LIVELO", CurrencyCode = "BRL" });
        context.MileagePurchases.AddRange(
            new MileagePurchase { Id = Guid.NewGuid(), UserId = userId, ProgramId = programId, AmountPaid = 300m, MilesPurchased = 10_000m, PurchaseDate = new DateOnly(2026, 8, 30) },
            new MileagePurchase { Id = Guid.NewGuid(), UserId = otherUserId, ProgramId = programId, AmountPaid = 500m, MilesPurchased = 20_000m, PurchaseDate = new DateOnly(2026, 8, 29) });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var config = new MapperConfiguration(cfg => cfg.CreateMap<MileagePurchase, MileagePurchaseDto>()
            .ForMember(d => d.ProgramName, o => o.MapFrom(s => s.Program.Name))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.Program.CurrencyCode)));
        var handler = new GetPurchasesHandler(context, config.CreateMapper(), new TestCurrentUser(userId));

        var result = await handler.Handle(new GetPurchasesQuery(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
        result.Value.Should().HaveCount(1);
        result.Value![0].MilesPurchased.Should().Be(10_000m);
        result.Value[0].AmountPaid.Should().Be(300m);
        result.Value[0].ProgramName.Should().Be("Livelo");
        result.Value[0].CurrencyCode.Should().Be("BRL");
    }

    [Fact]
    public async Task Handle_WhenUserHasNoPurchases_ReturnsEmptyCollection()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase($"GetPurchasesEmpty_{Guid.NewGuid()}").Options;
        await using var context = new MilesManagerDbContext(options);
        var config = new MapperConfiguration(cfg => cfg.CreateMap<MileagePurchase, MileagePurchaseDto>()
            .ForMember(d => d.ProgramName, o => o.MapFrom(s => s.Program.Name))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.Program.CurrencyCode)));
        var handler = new GetPurchasesHandler(context, config.CreateMapper(), new TestCurrentUser(Guid.NewGuid()));

        var result = await handler.Handle(new GetPurchasesQuery(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(MilesManager.Application.Common.ResultStatus.Success);
        result.Value.Should().NotBeNull().And.BeEmpty();
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