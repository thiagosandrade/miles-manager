using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Domain.Alerts;
using MilesManager.Infrastructure.Alerts;
using MilesManager.Infrastructure.Persistence;
using Xunit;

namespace MilesManager.Tests.Services;

public sealed class AlertIngestionServiceTests
{
    [Fact]
    public async Task IngestAllAsync_ExtractsTitleUrlDateChipsAndCountry()
    {
        const string html = """
            <html><body>
              <article>
                <h2><a href="/article-1">Azul oferece até 110% de bônus</a></h2>
                <time datetime="2026-09-17T10:00:00-03:00">há 1 hora</time>
                <a href="/categorias/promocoes/">Promoções</a>
                <a href="/categorias/transferencia-de-pontos/">Transferência de pontos</a>
              </article>
            </body></html>
            """;

        await using var context = CreateContext();
        context.AlertSources.Add(new AlertSource
        {
            Title = "Passageiro de Primeira",
            ExternalUrl = "https://passageirodeprimeira.com/categorias/promocoes/",
            Country = "Brazil"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var service = new AlertIngestionService(
            context,
            new HttpClient(new StubHttpMessageHandler(html)));

        var discovered = await service.IngestAllAsync(CancellationToken.None);

        discovered.Should().Be(1);
        var item = await context.AlertItems.SingleAsync(cancellationToken: TestContext.Current.CancellationToken);
        item.Title.Should().Be("Azul oferece até 110% de bônus");
        item.ExternalUrl.Should().Be("https://passageirodeprimeira.com/article-1");
        item.PublishedDate.Should().NotBeNull();
        item.Chips.Should().Be("Promoções, Transferência de pontos");
        item.Country.Should().Be("Brazil");
    }

    [Fact]
    public async Task IngestAllAsync_DoesNotDuplicateExistingUrl()
    {
        const string html = """
            <html><body>
              <article>
                <h2><a href="/article-1">Azul oferece até 110% de bônus</a></h2>
              </article>
            </body></html>
            """;

        await using var context = CreateContext();
        context.AlertSources.Add(new AlertSource
        {
            Title = "Blog",
            ExternalUrl = "https://example.com/promocoes",
            Country = "Brazil"
        });
        context.AlertItems.Add(new AlertItem
        {
            Title = "Azul oferece até 110% de bônus",
            ExternalUrl = "https://example.com/article-1",
            Country = "Brazil"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var service = new AlertIngestionService(
            context,
            new HttpClient(new StubHttpMessageHandler(html)));

        var discovered = await service.IngestAllAsync(CancellationToken.None);

        discovered.Should().Be(0);
        (await context.AlertItems.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
    }

    private static MilesManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MilesManagerDbContext(options);
    }

    private sealed class StubHttpMessageHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            });
        }
    }
}
