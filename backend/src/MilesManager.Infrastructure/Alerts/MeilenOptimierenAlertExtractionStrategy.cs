using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class MeilenOptimierenAlertExtractionStrategy : IAlertExtractionStrategy
{
    public bool CanHandle(Uri sourceUri) =>
        sourceUri.Host.Equals("meilenoptimieren.com", StringComparison.OrdinalIgnoreCase)
        || sourceUri.Host.Equals("www.meilenoptimieren.com", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri)
    {
        var sectionHeading = document
            .QuerySelectorAll("h1, h2, h3")
            .FirstOrDefault(x => IsOffersHeading(AlertExtractionHelpers.CleanText(x.TextContent)));

        if (sectionHeading is null)
        {
            yield break;
        }

        var headingLevel = GetHeadingLevel(sectionHeading);
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in document.All.SkipWhile(x => !ReferenceEquals(x, sectionHeading)))
        {
            if (ReferenceEquals(element, sectionHeading))
            {
                continue;
            }

            if (element.TagName.Equals($"H{headingLevel}", StringComparison.OrdinalIgnoreCase)
                || element.TagName.Equals($"H{Math.Max(1, headingLevel - 1)}", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (!element.TagName.StartsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var title = AlertExtractionHelpers.CleanText(element.TextContent);
            if (title.Length < 8 || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var link = FindArticleLink(element, sourceUri);
            var externalUrl = AlertExtractionHelpers.ResolveUrl(link?.GetAttribute("href"), sourceUri);

            if (!IsArticleUrl(externalUrl, sourceUri)
                || !seenUrls.Add(AlertExtractionHelpers.NormalizeUrl(externalUrl!)))
            {
                continue;
            }

            var card = element.Closest("article")
                ?? element.Closest("li")
                ?? element.ParentElement;

            if (card is null)
            {
                continue;
            }

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl!,
                PublishedDate = AlertExtractionHelpers.ExtractPublishedDate(card)
                    ?? ExtractVisibleDate(card),
                Chips = AlertExtractionHelpers.ExtractChips(card, externalUrl!)
            };
        }
    }

    private static IElement? FindArticleLink(IElement heading, Uri sourceUri)
    {
        var directLink = heading.Closest("a[href]");
        if (directLink is not null && IsArticleUrl(
            AlertExtractionHelpers.ResolveUrl(directLink.GetAttribute("href"), sourceUri),
            sourceUri))
        {
            return directLink;
        }

        var card = heading.Closest("article")
            ?? heading.Closest("li")
            ?? heading.ParentElement;

        if (card is null)
        {
            return null;
        }

        return card
            .QuerySelectorAll("a[href]")
            .FirstOrDefault(anchor =>
            {
                var href = AlertExtractionHelpers.ResolveUrl(anchor.GetAttribute("href"), sourceUri);
                if (!IsArticleUrl(href, sourceUri))
                {
                    return false;
                }

                var text = AlertExtractionHelpers.CleanText(anchor.TextContent);
                return string.Equals(text, AlertExtractionHelpers.CleanText(heading.TextContent), StringComparison.OrdinalIgnoreCase)
                    || anchor.QuerySelector("img") is not null;
            });
    }

    private static DateTimeOffset? ExtractVisibleDate(IElement element)
    {
        var text = AlertExtractionHelpers.CleanText(element.TextContent);

        var match = Regex.Match(
            text,
            @"(?<!\d)(\d{1,2})\.\s+([A-Za-zÄÖÜäöüß]+)\s+(\d{4})(?!\d)",
            RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            match = Regex.Match(
                text,
                @"(?<!\d)([A-Za-z]+)\s+(\d{1,2}),\s+(\d{4})(?!\d)",
                RegexOptions.CultureInvariant);
        }

        if (!match.Success)
        {
            return null;
        }

        var dateText = match.Groups[0].Value;

        if (DateTime.TryParse(
            dateText,
            CultureInfo.GetCultureInfo("de-DE"),
            DateTimeStyles.AllowWhiteSpaces,
            out var germanDate)
            || DateTime.TryParse(
                dateText,
                CultureInfo.GetCultureInfo("en-US"),
                DateTimeStyles.AllowWhiteSpaces,
                out germanDate))
        {
            return new DateTimeOffset(
                DateTime.SpecifyKind(germanDate.Date, DateTimeKind.Utc));
        }

        return null;
    }

    private static bool IsOffersHeading(string title) =>
        title.Equals("Aktuelle Angebote", StringComparison.OrdinalIgnoreCase)
        || title.Equals("Weitere aktuelle Angebote", StringComparison.OrdinalIgnoreCase);

    private static int GetHeadingLevel(IElement heading) =>
        int.TryParse(heading.TagName[1..], out var level) ? level : 2;

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

        var path = uri.AbsolutePath.TrimEnd('/');

        return !string.IsNullOrWhiteSpace(path)
            && !path.Equals(sourceUri.AbsolutePath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase)
            && !path.Equals("/aktuelle-angebote", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/category/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/tag/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/author/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/page/", StringComparison.OrdinalIgnoreCase);
    }
}
