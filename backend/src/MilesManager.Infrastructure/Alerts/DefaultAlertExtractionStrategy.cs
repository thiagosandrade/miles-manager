using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class DefaultAlertExtractionStrategy : IAlertExtractionStrategy
{
    public bool CanHandle(Uri sourceUri) => true;

    public IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var headings = document.QuerySelectorAll(
            "article h1, article h2, article h3, article h4, article h5, " +
            ".entry-title, .post-title, h2, h3, h4");

        foreach (var heading in headings)
        {
            var title = AlertExtractionHelpers.CleanText(heading.TextContent);
            if (string.IsNullOrWhiteSpace(title) || title.Length < 8 || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var card = heading.Closest("article")
                ?? heading.Closest("li")
                ?? heading.ParentElement;

            if (card is null)
            {
                continue;
            }

            var link = heading.Closest("a[href]")
                ?? card.QuerySelectorAll("a[href]")
                    .FirstOrDefault(anchor => string.Equals(
                        AlertExtractionHelpers.CleanText(anchor.TextContent),
                        title,
                        StringComparison.OrdinalIgnoreCase))
                ?? card.QuerySelector("a[href]");

            var externalUrl = AlertExtractionHelpers.ResolveUrl(link?.GetAttribute("href"), sourceUri);
            if (externalUrl is null || !seenUrls.Add(AlertExtractionHelpers.NormalizeUrl(externalUrl)))
            {
                continue;
            }

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl,
                PublishedDate = AlertExtractionHelpers.ExtractPublishedDate(card),
                Chips = AlertExtractionHelpers.ExtractChips(card, externalUrl)
            };
        }
    }
}

internal static class AlertExtractionHelpers
{
    public static DateTimeOffset? ExtractPublishedDate(IElement element)
    {
        var dateValue = element.QuerySelector("time[datetime]")?.GetAttribute("datetime")
            ?? element.QuerySelector("[itemprop='datePublished']")?.GetAttribute("datetime")
            ?? element.QuerySelector("[itemprop='datePublished']")?.GetAttribute("content")
            ?? element.QuerySelector("meta[property='article:published_time']")?.GetAttribute("content");

        return DateTimeOffset.TryParse(
            dateValue,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var date)
            ? date
            : null;
    }

    public static string? ExtractChips(IElement element, string itemUrl)
    {
        var links = element.QuerySelectorAll(
                "a[rel~='category'], .cat-links a, .post-categories a, " +
                ".entry-categories a, .entry-tags a, [class*='category'] a, [class*='tag'] a")
            .Concat(element.QuerySelectorAll("a[href*='/categorias/'], a[href*='/category/'], a[href*='/tags/']"))
            .Select(x => new
            {
                Text = CleanText(x.TextContent),
                Url = ResolveUrl(x.GetAttribute("href"), new Uri(itemUrl))
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

    public static string? ResolveUrl(string? href, Uri pageUri)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        if (Uri.TryCreate(href.Trim(), UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        return Uri.TryCreate(pageUri, href.Trim(), out var relative)
            ? relative.ToString()
            : null;
    }

    public static string NormalizeUrl(string url)
    {
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
        {
            return url.Trim().TrimEnd('/');
        }

        var builder = new UriBuilder(uri) { Fragment = string.Empty };
        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }

    public static string CleanText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Regex.Replace(value, @"\s+", " ").Trim();
    }

    public static bool IsNonArticleHeading(string title)
    {
        return title.Equals("Promoções", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Notícias", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Ranking de Cartões", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Central de Cupons", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Alertas Recentes", StringComparison.OrdinalIgnoreCase)
            || title.Equals("Publicações Recentes", StringComparison.OrdinalIgnoreCase);
    }
}
