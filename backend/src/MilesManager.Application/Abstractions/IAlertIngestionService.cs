namespace MilesManager.Application.Abstractions;

public interface IAlertIngestionService
{
    Task<int> IngestAllAsync(CancellationToken cancellationToken);

    Task<int> IngestSourceAsync(Guid sourceId, CancellationToken cancellationToken);
}
