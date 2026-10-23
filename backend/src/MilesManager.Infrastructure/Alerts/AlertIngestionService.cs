using AngleSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilesManager.Application.Abstractions;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class AlertIngestionService(
    IApplicationDbContext context,
    HttpClient httpClient,
    IEnumerable<IAlertExtractionStrategy> extractionStrategies,
    ILogger<AlertIngestionService> logger) : IAlertIngestionService
{
    public async Task<int> IngestAllAsync(CancellationToken cancellationToken)
    {
        var sources = await context.AlertSources
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var existingItems = await context.AlertItems
            .ToListAsync(cancellationToken);

        var existingByUrl = existingItems
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalUrl))
            .ToDictionary(x => AlertExtractionHelpers.NormalizeUrl(x.ExternalUrl), StringComparer.OrdinalIgnoreCase);

        var discovered = 0;

        foreach (var source in sources)
        {
            discovered += await IngestSourceAsync(source, existingByUrl, cancellationToken);
        }

        return discovered;
    }

    public async Task<int> IngestSourceAsync(Guid sourceId, CancellationToken cancellationToken)
    {
        var source = await context.AlertSources
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == sourceId, cancellationToken);

        if (source is null)
        {
            return 0;
        }

        var existingItems = await context.AlertItems
            .ToListAsync(cancellationToken);

        var existingByUrl = existingItems
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalUrl))
            .ToDictionary(x => AlertExtractionHelpers.NormalizeUrl(x.ExternalUrl), StringComparer.OrdinalIgnoreCase);

        return await IngestSourceAsync(source, existingByUrl, cancellationToken);
    }

    private async Task<int> IngestSourceAsync(
        AlertSource source,
        IDictionary<string, AlertItem> existingByUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Starting alert ingestion for source {SourceTitle} at {SourceUrl}",
                source.Title,
                source.ExternalUrl);

            if (!Uri.TryCreate(source.ExternalUrl, UriKind.Absolute, out var sourceUri))
            {
                logger.LogWarning(
                    "Skipping alert source {SourceTitle} because its URL is invalid: {SourceUrl}",
                    source.Title,
                    source.ExternalUrl);
                return 0;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, sourceUri);
            request.Headers.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

            using var response = await httpClient.SendAsync(request, cancellationToken);
            logger.LogInformation(
                "Received HTTP {StatusCode} for alert source {SourceTitle} at {SourceUrl}; content type {ContentType}",
                (int)response.StatusCode,
                source.Title,
                source.ExternalUrl,
                response.Content.Headers.ContentType?.ToString() ?? "(unknown)");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogInformation(
                "Downloaded {HtmlLength} characters for alert source {SourceTitle} at {SourceUrl}",
                html.Length,
                source.Title,
                source.ExternalUrl);
            var document = await BrowsingContext.New(Configuration.Default).OpenAsync(
                request => request.Content(html).Address(sourceUri),
                cancellationToken);

            var strategy = extractionStrategies.FirstOrDefault(x => x.CanHandle(sourceUri));
            if (strategy is null)
            {
                logger.LogWarning(
                    "No alert extraction strategy is registered for source {SourceTitle} at {SourceUrl}",
                    source.Title,
                    source.ExternalUrl);
                return 0;
            }

            var extractedItems = strategy.ExtractItems(document, sourceUri).ToList();
            var discovered = 0;
            var duplicateCount = 0;
            var unusableCount = 0;

            logger.LogInformation(
                "Alert extraction strategy {StrategyName} found {ExtractedCount} candidate item(s) for source {SourceTitle} at {SourceUrl}",
                strategy.GetType().Name,
                extractedItems.Count,
                source.Title,
                source.ExternalUrl);

            foreach (var item in extractedItems)
            {
                if (!IsUsableAlertUrl(item.ExternalUrl))
                {
                    logger.LogWarning(
                        "Ignoring extracted alert with an unusable URL from source {SourceTitle}: {AlertUrl}",
                        source.Title,
                        item.ExternalUrl);
                    unusableCount++;
                    continue;
                }

                item.Country = source.Country.Trim();

                var key = AlertExtractionHelpers.NormalizeUrl(item.ExternalUrl);
                if (existingByUrl.TryGetValue(key, out var existing))
                {
                    var changed = false;

                    if (!string.Equals(existing.Title, item.Title, StringComparison.Ordinal))
                    {
                        existing.Title = item.Title;
                        changed = true;
                    }

                    if (existing.PublishedDate != item.PublishedDate)
                    {
                        existing.PublishedDate = item.PublishedDate;
                        changed = true;
                    }

                    if (!string.Equals(existing.Chips, item.Chips, StringComparison.Ordinal))
                    {
                        existing.Chips = item.Chips;
                        changed = true;
                    }

                    if (!string.Equals(existing.Country, item.Country, StringComparison.Ordinal))
                    {
                        existing.Country = item.Country;
                        changed = true;
                    }

                    if (changed)
                    {
                        existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
                    }

                    duplicateCount++;
                    continue;
                }

                context.AlertItems.Add(item);
                existingByUrl[key] = item;
                discovered++;
            }

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Alert ingestion completed for source {SourceTitle} at {SourceUrl}: {ExtractedCount} candidate(s), {DuplicateCount} existing URL(s), {UnusableCount} unusable URL(s), {DiscoveredCount} new alert(s)",
                source.Title,
                source.ExternalUrl,
                extractedItems.Count,
                duplicateCount,
                unusableCount,
                discovered);

            return discovered;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Alert ingestion failed for source {SourceTitle} at {SourceUrl}",
                source.Title,
                source.ExternalUrl);
            return 0;
        }
    }

    private static bool IsUsableAlertUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && string.IsNullOrWhiteSpace(uri.Fragment);
    }
}
