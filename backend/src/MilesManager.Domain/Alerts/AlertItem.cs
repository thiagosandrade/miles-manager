using MilesManager.Domain.Common;

namespace MilesManager.Domain.Alerts;

public sealed class AlertItem : Entity
{
    public string Title { get; set; } = string.Empty;
    public string ExternalUrl { get; set; } = string.Empty;
    public DateTimeOffset? PublishedDate { get; set; }
    public string? Chips { get; set; }
    public string Country { get; set; } = string.Empty;
}
