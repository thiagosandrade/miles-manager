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

        foreach (var anchor in document.QuerySelectorAll("main a[href]"))
        {
            var title = AlertExtractionHelpers.CleanText(anchor.TextContent);
            var externalUrl = AlertExtractionHelpers.ResolveUrl(anchor.GetAttribute("href"), sourceUri);

            if (title.Length < 12
                || !IsArticleUrl(externalUrl, sourceUri)
                || !seenUrls.Add(AlertExtractionHelpers.NormalizeUrl(externalUrl!)))
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

            var publishedDate = AlertExtractionHelpers.ExtractPublishedDate(card)
                ?? ExtractVisibleDate(card);

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl!,
                PublishedDate = publishedDate,
                Chips = "Credit Cards"
            };
        }
    }

    private static bool IsArticleUrl(string? url, Uri sourceUri)
    {
        if (url is null || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!uri.Host.Equals(sourceUri.Host, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var path = uri.AbsolutePath.Trim('/');
        return path.StartsWith("news/", StringComparison.OrdinalIgnoreCase)
            && !path.Equals("news/credit-cards", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("news/credit-cards/page/", StringComparison.OrdinalIgnoreCase);
    }

    private static DateTimeOffset? ExtractVisibleDate(IElement card)
    {
        var text = AlertExtractionHelpers.CleanText(card.TextContent);
        var match = System.Text.RegularExpressions.Regex.Match(
            text,
            @"\b(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\.?\s+\d{1,2},\s+\d{4}\b",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return match.Success
            && DateTimeOffset.TryParse(match.Value, System.Globalization.CultureInfo.InvariantCulture, out var date)
            ? date
            : null;
    }
}
