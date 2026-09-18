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

        foreach (var sectionHeading in document.QuerySelectorAll("h1, h2, h3")
                     .Where(IsCreditCardNewsSection))
        {
            var headingLevel = GetHeadingLevel(sectionHeading);

            foreach (var element in document.All.SkipWhile(x => !ReferenceEquals(x, sectionHeading)))
            {
                if (ReferenceEquals(element, sectionHeading))
                {
                    continue;
                }

                if (element is IElement current
                    && IsHeadingAtOrAboveLevel(current, headingLevel))
                {
                    break;
                }

                if (element is not IElement anchor
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
                if (title.Length < 12 || AlertExtractionHelpers.IsNonArticleHeading(title))
                {
                    continue;
                }

                var normalizedUrl = AlertExtractionHelpers.NormalizeUrl(externalUrl!);
                if (!seenUrls.Add(normalizedUrl))
                {
                    continue;
                }

                var publishedDate = FindPublishedDate(anchor);

                yield return new AlertItem
                {
                    Title = title,
                    ExternalUrl = externalUrl!,
                    PublishedDate = publishedDate,
                    Chips = "Credit Cards"
                };
            }
        }
    }

    private static bool IsCreditCardNewsSection(IElement element)
    {
        var text = AlertExtractionHelpers.CleanText(element.TextContent);

        return text.Equals("Latest credit card news", StringComparison.OrdinalIgnoreCase)
            || text.Equals("More credit card news", StringComparison.OrdinalIgnoreCase);
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

    private static bool IsHeadingAtOrAboveLevel(IElement element, int level)
    {
        if (!element.TagName.StartsWith("H", StringComparison.OrdinalIgnoreCase)
            || !int.TryParse(element.TagName[1..], out var elementLevel))
        {
            return false;
        }

        return elementLevel <= level;
    }

    private static int GetHeadingLevel(IElement heading) =>
        int.TryParse(heading.TagName[1..], out var level) ? level : 2;

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