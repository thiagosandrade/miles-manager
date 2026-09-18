using System.Globalization;
using System.Text.RegularExpressions;
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

            if (!IsArticleUrl(externalUrl, sourceUri)
                || !seenUrls.Add(AlertExtractionHelpers.NormalizeUrl(externalUrl!)))
            {
                continue;
            }

            var title = AlertExtractionHelpers.CleanText(anchor.TextContent);
            if (title.Length < 12)
            {
                var heading = anchor.Closest("h1, h2, h3, h4, h5, h6");
                title = AlertExtractionHelpers.CleanText(heading?.TextContent);
            }

            if (title.Length < 12)
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

        if (!uri.Host.Equals(sourceUri.Host, StringComparison.OrdinalIgnoreCase)
            && !uri.Host.Equals("thepointsguy.com", StringComparison.OrdinalIgnoreCase)
            && !uri.Host.Equals("www.thepointsguy.com", StringComparison.OrdinalIgnoreCase))
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

        var match = Regex.Match(
            text,
            @"\b(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\.?\s+\d{1,2},\s+\d{4}\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        return match.Success
            && DateTimeOffset.TryParse(
                match.Value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var date)
            ? date.ToUniversalTime()
            : null;
    }
}
