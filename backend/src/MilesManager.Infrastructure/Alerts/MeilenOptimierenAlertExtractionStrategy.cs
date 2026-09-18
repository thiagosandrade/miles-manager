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
            .QuerySelectorAll("h2, h3")
            .FirstOrDefault(x =>
                AlertExtractionHelpers.CleanText(x.TextContent)
                    .Equals("Alle Artikel zum Meilen sammeln", StringComparison.OrdinalIgnoreCase));

        if (sectionHeading is null)
        {
            yield break;
        }

        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var headingLevel = GetHeadingLevel(sectionHeading);

        foreach (var element in document.All.SkipWhile(x => !ReferenceEquals(x, sectionHeading)))
        {
            if (ReferenceEquals(element, sectionHeading))
            {
                continue;
            }

            if (IsHeadingAtOrAboveLevel(element, headingLevel))
            {
                break;
            }

            if (element is not IElement current
                || !current.TagName.Equals("A", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var externalUrl = AlertExtractionHelpers.ResolveUrl(
                current.GetAttribute("href"),
                sourceUri);

            if (!IsArticleUrl(externalUrl, sourceUri))
            {
                continue;
            }

            var title = AlertExtractionHelpers.CleanText(current.TextContent);
            if (title.Length < 20
                || title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 4
                || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var normalizedUrl = AlertExtractionHelpers.NormalizeUrl(externalUrl!);
            if (!seenUrls.Add(normalizedUrl))
            {
                continue;
            }

            var card = current.Closest("article")
                ?? current.Closest("li")
                ?? current.ParentElement;

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
                Chips = AlertExtractionHelpers.ExtractChips(card, externalUrl!)
            };
        }
    }

    private static DateTimeOffset? ExtractVisibleDate(IElement element)
    {
        var text = AlertExtractionHelpers.CleanText(element.TextContent);

        var numericMatch = Regex.Match(
            text,
            @"(?<!\d)(\d{1,2})\.\s*(\d{1,2})\.\s*(\d{4})(?!\d)",
            RegexOptions.CultureInvariant);

        if (numericMatch.Success
            && DateTime.TryParseExact(
                numericMatch.Value,
                new[] { "dd.MM.yyyy", "d.M.yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var numericDate))
        {
            return new DateTimeOffset(DateTime.SpecifyKind(numericDate.Date, DateTimeKind.Utc));
        }

        var germanMatch = Regex.Match(
            text,
            @"(?<!\d)(\d{1,2})\.\s+([A-Za-zÄÖÜäöüß]+)\s+(\d{4})(?!\d)",
            RegexOptions.CultureInvariant);

        if (germanMatch.Success
            && DateTime.TryParse(
                germanMatch.Value,
                CultureInfo.GetCultureInfo("de-DE"),
                DateTimeStyles.AllowWhiteSpaces,
                out var germanDate))
        {
            return new DateTimeOffset(DateTime.SpecifyKind(germanDate.Date, DateTimeKind.Utc));
        }

        var englishMatch = Regex.Match(
            text,
            @"(?<!\d)([A-Za-z]+)\.?\s+(\d{1,2}),\s+(\d{4})(?!\d)",
            RegexOptions.CultureInvariant);

        if (englishMatch.Success
            && DateTime.TryParse(
                englishMatch.Value,
                CultureInfo.GetCultureInfo("en-US"),
                DateTimeStyles.AllowWhiteSpaces,
                out var englishDate))
        {
            return new DateTimeOffset(DateTime.SpecifyKind(englishDate.Date, DateTimeKind.Utc));
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
            && !path.StartsWith("/page/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/comments/", StringComparison.OrdinalIgnoreCase)
            && !uri.Fragment.Contains("comments", StringComparison.OrdinalIgnoreCase);
    }
}
