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

        // The category page can render article titles as peer-level headings, and
        // its sections can appear in either order. Don't depend on heading levels
        // to determine whether a link belongs to a section; classify the actual
        // article URL instead.
        foreach (var anchor in document.QuerySelectorAll("a[href]"))
        {
            var externalUrl = AlertExtractionHelpers.ResolveUrl(
                anchor.GetAttribute("href"),
                sourceUri);

            if (!IsArticleUrl(externalUrl))
            {
                continue;
            }

            var title = AlertExtractionHelpers.CleanText(anchor.TextContent);

            if (title.Length < 12)
            {
                var titleHeading = anchor.Closest("h1, h2, h3, h4, h5, h6");
                title = AlertExtractionHelpers.CleanText(titleHeading?.TextContent);
            }

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

        // Article URLs have the form /news/{article-slug}/. Category links
        // such as /news/credit-cards/ and nested category URLs are not articles.
        return segments.Length == 2
            && segments[0].Equals("news", StringComparison.OrdinalIgnoreCase)
            && !NewsSectionSlugs.Contains(segments[1]);
    }
}
