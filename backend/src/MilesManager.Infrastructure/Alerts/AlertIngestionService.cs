using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class AlertIngestionService(
    IApplicationDbContext context,
    HttpClient httpClient) : IAlertIngestionService
{
    public async Task<int> IngestAllAsync(CancellationToken cancellationToken)
    {
        var sources = await context.AlertSources
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var existingItems = await context.AlertItems
            .ToListAsync(cancellationToken);

        var existingByUrl = existingItems
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalUrl))
            .ToDictionary(x => NormalizeUrl(x.ExternalUrl), StringComparer.OrdinalIgnoreCase);

        var discovered = 0;

        foreach (var source in sources)
        {
            try
            {
                var html = await httpClient.GetStringAsync(source.ExternalUrl, cancellationToken);
                var document = await BrowsingContext.New(Configuration.Default).OpenAsync(
                    request => request.Content(html).Address(source.ExternalUrl),
                    cancellationToken);

                foreach (var item in ExtractItems(document, source.ExternalUrl))
                {
                    var key = NormalizeUrl(item.ExternalUrl);
                    if (existingByUrl.TryGetValue(key, out var existing))
                    {
                        var changed = false;

                        if (!string.Equals(existing.Title, item.Title, StringComparison.Ordinal))
                        {
                            existing.Title = item.Title;
                            changed = true;
                        }

                        if (existing.PublishedDate != item.PublishedDate)
                        {
                            existing.PublishedDate = item.PublishedDate;
                            changed = true;
                        }

                        if (!string.Equals(existing.Chips, item.Chips, StringComparison.Ordinal))
                        {
                            existing.Chips = item.Chips;
                            changed = true;
                        }

                        if (changed)
                        {
                            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
                        }

                        continue;
                    }

                    context.AlertItems.Add(item);
                    existingByUrl[key] = item;
                    discovered++;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // A failing source must not prevent the remaining alert sources from being ingested.
            }
        }

        return discovered;
    }

    private static IEnumerable<AlertItem> ExtractItems(IDocument document, string sourceUrl)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var headings = document.QuerySelectorAll(
            "article h1, article h2, article h3, article h4, article h5, " +
            ".entry-title, .post-title, h2, h3, h4");

        foreach (var heading in headings)
        {
            var title = CleanText(heading.TextContent);
            if (string.IsNullOrWhiteSpace(title) || title.Length < 8 || IsNonArticleHeading(title))
            {
                continue;
            }

            var card = FindCard(heading);
            if (card is null)
            {
                continue;
            }

            var link = FindItemLink(card, heading, title);
            var externalUrl = ResolveUrl(link?.GetAttribute("href"), sourceUrl);
            if (externalUrl is null || !seenUrls.Add(NormalizeUrl(externalUrl)))
            {
                continue;
            }

            var publishedDate = ExtractPublishedDate(card);
            var chips = ExtractChips(card, externalUrl);

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl,
                PublishedDate = publishedDate,
                Chips = chips
            };
        }
    }

    private static IElement? FindCard(IElement heading)
    {
        return heading.Closest("article")
            ?? heading.Closest("li")
            ?? heading.ParentElement;
    }

    private static IElement? FindItemLink(IElement card, IElement heading, string title)
    {
        var closest = heading.Closest("a[href]");
        if (closest is not null)
        {
            return closest;
        }

        return card.QuerySelectorAll("a[href]")
            .FirstOrDefault(anchor => string.Equals(
                CleanText(anchor.TextContent),
                title,
                StringComparison.OrdinalIgnoreCase))
            ?? card.QuerySelector("a[href]");
    }

    private static DateTimeOffset? ExtractPublishedDate(IElement card)
    {
        var dateValue = card.QuerySelector("time[datetime]")?.GetAttribute("datetime")
            ?? card.QuerySelector("[itemprop='datePublished']")?.GetAttribute("datetime")
            ?? card.QuerySelector("[itemprop='datePublished']")?.GetAttribute("content")
            ?? card.QuerySelector("meta[property='article:published_time']")?.GetAttribute("content");

        return DateTimeOffset.TryParse(
            dateValue,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var date)
            ? date
            : null;
    }

    private static string? ExtractChips(IElement card, string itemUrl)
    {
        var links = card.QuerySelectorAll(
                "a[rel~='category'], .cat-links a, .post-categories a, " +
                ".entry-categories a, .entry-tags a, [class*='category'] a, [class*='tag'] a")
            .Concat(card.QuerySelectorAll("a[href*='/categorias/'], a[href*='/category/'], a[href*='/tags/']"))
            .Select(x => new
            {
                Text = CleanText(x.TextContent),
                Url = ResolveUrl(x.GetAttribute("href"), itemUrl)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Text)
                && x.Text.Length <= 100
                && x.Url is not null
                && !string.Equals(NormalizeUrl(x.Url), NormalizeUrl(itemUrl), StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Text)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return links.Length == 0 ? null : string.Join(", ", links);
    }

    private static bool IsNonArticleHeading(string title)
    {
        return title.Equals("Promoções", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Notícias", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Ranking de Cartões", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Central de Cupons", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Alertas Recentes", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Publicações Recentes", StringComparison.OrdinalIgnoreCase);
    }

    private static string? ResolveUrl(string? href, string pageUrl)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        if (Uri.TryCreate(href.Trim(), UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        return Uri.TryCreate(new Uri(pageUrl), href.Trim(), out var relative)
            ? relative.ToString()
            : null;
    }

    private static string NormalizeUrl(string url)
    {
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
        {
            return url.Trim().TrimEnd('/');
        }

        var builder = new UriBuilder(uri) { Fragment = string.Empty };
        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }

    private static string CleanText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var cleaned = Regex.Replace(value, @"\s+", " ");
        return cleaned.Trim();
    }
}
