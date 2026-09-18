namespace MilesManager.Application.Abstractions;

public interface IAlertIngestionService
{
    Task<int> IngestAllAsync(CancellationToken cancellationToken);
}
