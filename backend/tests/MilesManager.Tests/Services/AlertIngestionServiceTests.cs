using System.Net;
using AngleSharp;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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

        var service = CreateService(context, html, [new DefaultAlertExtractionStrategy()]);

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

        var service = CreateService(context, html, [new DefaultAlertExtractionStrategy()]);

        var discovered = await service.IngestAllAsync(CancellationToken.None);

        discovered.Should().Be(0);
        (await context.AlertItems.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
    }

    [Fact]
    public async Task ThePointsGuyStrategy_ExtractsPeerLevelArticleHeadingsAndExcludesCategoryLinks()
    {
        const string html = """
            <html><body>
              <h2>More credit card news</h2>
              <article>
                <h2><a href="/news/amex-platinum-airbnb-hotel-credit-changes/">
                  Amex Platinum adds Airbnb perks and credits, ends one-night hotel credits
                </a></h2>
                <time datetime="2026-10-22T10:00:00Z">October 22</time>
              </article>
              <a href="/news/credit-cards/">Credit cards</a>
              <a href="/news/airlines/">Airlines</a>
              <a href="/news/credit-cards/some-subcategory/">Nested category</a>
              <h2>Latest credit card news</h2>
              <article>
                <h2><a href="/news/best-cards-for-holiday-shopping-this-year/">
                  Best credit cards for holiday shopping this year
                </a></h2>
              </article>
            </body></html>
            """;

        var sourceUri = new Uri("https://thepointsguy.com/news/credit-cards/");
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(request => request.Content(html).Address(sourceUri), cancel: TestContext.Current.CancellationToken);
        var strategy = new ThePointsGuyAlertExtractionStrategy();

        var items = strategy.ExtractItems(document, sourceUri).ToList();

        items.Should().HaveCount(2);
        items.Select(x => x.ExternalUrl).Should().Contain(
            "https://thepointsguy.com/news/amex-platinum-airbnb-hotel-credit-changes/");
        items.Select(x => x.ExternalUrl).Should().Contain(
            "https://thepointsguy.com/news/best-cards-for-holiday-shopping-this-year/");
        items.Should().OnlyContain(x => x.Chips == "Credit Cards");
        items.Single(x => x.ExternalUrl.Contains("amex-platinum", StringComparison.Ordinal))
            .PublishedDate.Should().Be(DateTimeOffset.Parse("2026-10-22T10:00:00Z"));
    }

    [Fact]
    public async Task IngestAllAsync_LogsAndDoesNotThrowWhenSourceReturnsAnError()
    {
        await using var context = CreateContext();
        context.AlertSources.Add(new AlertSource
        {
            Title = "The Points Guy",
            ExternalUrl = "https://thepointsguy.com/news/credit-cards/",
            Country = "United States"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var service = new AlertIngestionService(
            context,
            new HttpClient(new StubHttpMessageHandler("<html></html>", HttpStatusCode.InternalServerError)),
            [new ThePointsGuyAlertExtractionStrategy()],
            NullLogger<AlertIngestionService>.Instance);

        var action = () => service.IngestAllAsync(CancellationToken.None);

        await action.Should().NotThrowAsync();
        (await context.AlertItems.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
    }

    private static AlertIngestionService CreateService(
        MilesManagerDbContext context,
        string html,
        IEnumerable<IAlertExtractionStrategy> strategies) =>
        new(
            context,
            new HttpClient(new StubHttpMessageHandler(html)),
            strategies,
            NullLogger<AlertIngestionService>.Instance);

    private static MilesManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MilesManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MilesManagerDbContext(options);
    }

    private sealed class StubHttpMessageHandler(
        string content,
        HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            });
        }
    }
}
