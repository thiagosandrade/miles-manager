using MilesManager.Domain.Common;

namespace MilesManager.Domain.Users;

public sealed class UserPreferences : Entity
{
    public Guid UserId { get; set; }
    public string PreferredCurrency { get; set; } = "USD";
    public string? Theme { get; set; } = "Light";
    public bool EmailNotificationsEnabled { get; set; } = true;
    public bool ExpirationAlertsEnabled { get; set; } = true;
}
