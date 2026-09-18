using AngleSharp;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public sealed class AlertIngestionService(
    IApplicationDbContext context,
    HttpClient httpClient,
    IEnumerable<IAlertExtractionStrategy> extractionStrategies) : IAlertIngestionService
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
            try
            {
                if (!Uri.TryCreate(source.ExternalUrl, UriKind.Absolute, out var sourceUri))
                {
                    continue;
                }

                var html = await httpClient.GetStringAsync(sourceUri, cancellationToken);
                var document = await BrowsingContext.New(Configuration.Default).OpenAsync(
                    request => request.Content(html).Address(sourceUri),
                    cancellationToken);

                var strategy = extractionStrategies.FirstOrDefault(x => x.CanHandle(sourceUri));
                if (strategy is null)
                {
                    continue;
                }

                foreach (var item in strategy.ExtractItems(document, sourceUri))
                {
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

                        if (changed)
                        {
                            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
                        }

                        continue;
                    }

                    context.AlertItems.Add(item);
                    existingByUrl[key] = item;
                    discovered++;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // A failing source must not prevent the remaining alert sources from being ingested.
            }
        }

        return discovered;
    }
}
