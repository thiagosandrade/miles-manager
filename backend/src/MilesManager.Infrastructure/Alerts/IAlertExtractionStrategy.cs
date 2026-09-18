using AngleSharp.Dom;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Alerts;

public interface IAlertExtractionStrategy
{
    bool CanHandle(Uri sourceUri);

    IEnumerable<AlertItem> ExtractItems(IDocument document, Uri sourceUri);
}
