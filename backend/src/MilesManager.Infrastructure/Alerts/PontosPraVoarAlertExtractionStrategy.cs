using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class PontosPraVoarAlertExtractionStrategy : IAlertExtractionStrategy
{
    public bool CanHandle(Uri sourceUri) =>
        sourceUri.Host.Equals("pontospravoar.com", StringComparison.OrdinalIgnoreCase)
        || sourceUri.Host.Equals("www.pontospravoar.com", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var heading in document.QuerySelectorAll("h2, h3, h4"))
        {
            var title = AlertExtractionHelpers.CleanText(heading.TextContent);
            if (title.Length < 8 || AlertExtractionHelpers.IsNonArticleHeading(title))
            {
                continue;
            }

            var link = heading.QuerySelector("a[href]") ?? heading.Closest("a[href]");
            var externalUrl = AlertExtractionHelpers.ResolveUrl(link?.GetAttribute("href"), sourceUri);
            if (!IsArticleUrl(externalUrl, sourceUri) || !seenUrls.Add(AlertExtractionHelpers.NormalizeUrl(externalUrl!)))
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

            yield return new AlertItem
            {
                Title = title,
                ExternalUrl = externalUrl!,
                PublishedDate = AlertExtractionHelpers.ExtractPublishedDate(card),
                Chips = AlertExtractionHelpers.ExtractChips(card, externalUrl!)
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

        return !uri.AbsolutePath.Equals(sourceUri.AbsolutePath, StringComparison.OrdinalIgnoreCase)
            && !uri.AbsolutePath.Equals("/", StringComparison.OrdinalIgnoreCase)
            && !uri.AbsolutePath.Contains("/category/", StringComparison.OrdinalIgnoreCase)
            && !uri.AbsolutePath.Contains("/categorias/", StringComparison.OrdinalIgnoreCase);
    }
}
