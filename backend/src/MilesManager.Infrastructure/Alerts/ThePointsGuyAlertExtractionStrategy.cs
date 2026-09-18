using System.Globalization;
using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class ThePointsGuyAlertExtractionStrategy : IAlertExtractionStrategy
{
    public bool CanHandle(Uri sourceUri) =>
        sourceUri.Host.Equals("thepointsguy.com", StringComparison.OrdinalIgnoreCase)
        || sourceUri.Host.Equals("www.thepointsguy.com", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var anchor in document.QuerySelectorAll("a[href]"))
        {
            var externalUrl = AlertExtractionHelpers.ResolveUrl(anchor.GetAttribute("href"), sourceUri);

            if (!IsArticleUrl(externalUrl, sourceUri))
            {
                continue;
            }

            var title = AlertExtractionHelpers.CleanText(anchor.TextContent);
            if (title.Length < 12)
            {
                var heading = anchor.Closest("h1, h2, h3, h4, h5, h6");
                title = AlertExtractionHelpers.CleanText(heading?.TextContent);
            }

            if (title.Length < 12 || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var card = anchor.Closest("article")
                ?? anchor.Closest("li")
                ?? anchor.ParentElement;

            if (card is null)
            {
                continue;
            }

            var publishedDate = AlertExtractionHelpers.ExtractPublishedDate(card);
            if (publishedDate is null)
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
                PublishedDate = publishedDate,
                Chips = AlertExtractionHelpers.ExtractChips(card, externalUrl!)
                    ?? "Credit Cards"
            };
        }
    }

    private static bool IsArticleUrl(string? url, Uri sourceUri)
    {
        if (url is null || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!uri.Host.Equals(sourceUri.Host, StringComparison.OrdinalIgnoreCase)
            && !uri.Host.Equals("thepointsguy.com", StringComparison.OrdinalIgnoreCase)
            && !uri.Host.Equals("www.thepointsguy.com", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var path = uri.AbsolutePath.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(path)
            || path.Equals(sourceUri.AbsolutePath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !path.StartsWith("/author/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/category/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/tag/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/search/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/page/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/about", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/contact", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/privacy", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/terms", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/newsletter", StringComparison.OrdinalIgnoreCase);
    }
}
