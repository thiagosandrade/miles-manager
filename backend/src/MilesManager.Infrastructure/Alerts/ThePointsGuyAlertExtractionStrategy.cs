using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class ThePointsGuyAlertExtractionStrategy : IAlertExtractionStrategy
{
    private static readonly HashSet<string> NewsSectionSlugs = new(StringComparer.OrdinalIgnoreCase)
    {
        "airlines",
        "aviation",
        "credit-cards",
        "cruises",
        "deals",
        "hotels"
    };

    public bool CanHandle(Uri sourceUri) =>
        sourceUri.Host.Equals("thepointsguy.com", StringComparison.OrdinalIgnoreCase)
        || sourceUri.Host.Equals("www.thepointsguy.com", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Classify links by their URLs instead of relying on heading levels,
        // because article sections may render titles as peer-level headings
        // or as accessible labels on image-only links.
        foreach (var anchor in document.QuerySelectorAll("a[href]"))
        {
            var externalUrl = AlertExtractionHelpers.ResolveUrl(
                anchor.GetAttribute("href"),
                sourceUri);

            if (!IsArticleUrl(externalUrl))
            {
                continue;
            }

            var title = ExtractTitle(anchor);

            if (title.Length < 12 || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var normalizedUrl = AlertExtractionHelpers.NormalizeUrl(externalUrl!);

            if (!seenUrls.Add(normalizedUrl))
            {
                continue;
            }

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl!,
                PublishedDate = FindPublishedDate(anchor),
                Chips = "Credit Cards"
            };
        }
    }

    private static string ExtractTitle(IElement anchor)
    {
        // Prefer visible link text when available.
        var title = AlertExtractionHelpers.CleanText(anchor.TextContent);

        if (title.Length >= 12)
        {
            return title;
        }

        // Some TPG article links contain only an image. In these cases,
        // the article title is provided through the accessible aria-label.
        title = AlertExtractionHelpers.CleanText(
            anchor.GetAttribute("aria-label"));

        if (title.Length >= 12)
        {
            return title;
        }

        // Fall back to an associated heading when the anchor is inside one.
        var titleHeading = anchor.Closest("h1, h2, h3, h4, h5, h6");
        title = AlertExtractionHelpers.CleanText(titleHeading?.TextContent);

        if (title.Length >= 12)
        {
            return title;
        }

        // Some layouts place the heading next to the anchor rather than
        // inside it. Check the nearest parent for a heading as a final fallback.
        var parentHeading = anchor.ParentElement?
            .QuerySelector("h1, h2, h3, h4, h5, h6");

        return AlertExtractionHelpers.CleanText(parentHeading?.TextContent);
    }

    private static DateTimeOffset? FindPublishedDate(IElement anchor)
    {
        var current = anchor;

        for (var level = 0; level < 6 && current is not null; level++, current = current.ParentElement)
        {
            var date = AlertExtractionHelpers.ExtractPublishedDate(current);

            if (date is not null)
            {
                return date;
            }
        }

        return null;
    }

    private static bool IsArticleUrl(string? url)
    {
        if (url is null || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!uri.Host.Equals("thepointsguy.com", StringComparison.OrdinalIgnoreCase)
            && !uri.Host.Equals("www.thepointsguy.com", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var segments = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Article URLs have the form /news/{article-slug}.
        // Exclude category pages and nested category URLs.
        return segments.Length == 2
            && segments[0].Equals("news", StringComparison.OrdinalIgnoreCase)
            && !NewsSectionSlugs.Contains(segments[1]);
    }
}
