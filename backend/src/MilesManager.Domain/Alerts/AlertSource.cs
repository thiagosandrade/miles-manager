using MilesManager.Domain.Common;

namespace MilesManager.Domain.Alerts;

public sealed class AlertSource : Entity
{
    public string Title { get; set; } = string.Empty;
    public string ExternalUrl { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
