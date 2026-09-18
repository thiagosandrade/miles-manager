using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MilesManager.Application.Abstractions;

namespace MilesManager.Infrastructure.Alerts;

public sealed class AlertIngestionHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<AlertIngestionHostedService> logger,
    IConfiguration configuration) : BackgroundService
{
    private const int RunHour = 10;
    private const int RunMinute = 0;
    private const string BrazilTimeZoneId = "America/Sao_Paulo";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue<bool>("Alerts:IngestionJobEnabled"))
        {
            logger.LogInformation("Alert ingestion job is disabled.");
            return;
        }

        await RunIngestionAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun(DateTimeOffset.UtcNow);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await RunIngestionAsync(stoppingToken);
        }
    }

    private async Task RunIngestionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var ingestionService = scope.ServiceProvider.GetRequiredService<IAlertIngestionService>();
            var discovered = await ingestionService.IngestAllAsync(cancellationToken);

            logger.LogInformation(
                "Alert ingestion completed. {DiscoveredCount} new alert items discovered.",
                discovered);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Alert ingestion failed.");
        }
    }

    private static TimeSpan GetDelayUntilNextRun(DateTimeOffset utcNow)
    {
        var timeZone = GetBrazilTimeZone();
        var localNow = TimeZoneInfo.ConvertTime(utcNow, timeZone);
        var nextRunLocal = localNow.Date.AddDays(localNow.TimeOfDay >= TimeSpan.FromHours(RunHour) ? 1 : 0)
            .AddHours(RunHour)
            .AddMinutes(RunMinute);

        var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRunLocal, timeZone);
        return nextRunUtc - utcNow.UtcDateTime;
    }

    private static TimeZoneInfo GetBrazilTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(BrazilTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }
}
