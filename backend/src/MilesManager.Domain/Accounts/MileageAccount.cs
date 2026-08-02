using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Accounts;

public sealed class MileageAccount : Entity
{
    public Guid ProgramId { get; set; }
    public Guid? UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal CurrentBalanceMiles { get; set; }
    public decimal PendingMiles { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateOnly? ExpirationDate { get; set; }
}
