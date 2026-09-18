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
        var activeSectionLevel = (int?)null;

        foreach (var element in document.All)
        {
            if (element is IElement heading && IsHeading(heading))
            {
                var level = GetHeadingLevel(heading);
                var text = AlertExtractionHelpers.CleanText(heading.TextContent);

                if (IsCreditCardNewsSection(text))
                {
                    activeSectionLevel = level;
                    continue;
                }

                continue;
            }

            if (activeSectionLevel is null
                || element is not IElement anchor
                || !anchor.TagName.Equals("A", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var externalUrl = AlertExtractionHelpers.ResolveUrl(
                anchor.GetAttribute("href"),
                sourceUri);

            if (!IsArticleUrl(externalUrl, sourceUri))
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

    private static bool IsCreditCardNewsSection(string text) =>
        text.Equals("Latest credit card news", StringComparison.OrdinalIgnoreCase)
        || text.Equals("More credit card news", StringComparison.OrdinalIgnoreCase);

    private static bool IsHeading(IElement element) =>
        element.TagName.Length == 2
        && element.TagName[0] == 'H'
        && char.IsDigit(element.TagName[1]);

    private static int GetHeadingLevel(IElement heading) =>
        int.TryParse(heading.TagName[1..], out var level) ? level : 6;

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

        return path.StartsWith("/news/credit-cards/", StringComparison.OrdinalIgnoreCase)
            && !path.Equals("/news/credit-cards", StringComparison.OrdinalIgnoreCase);
    }
}