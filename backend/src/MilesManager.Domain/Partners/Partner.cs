using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Domain.Partners;

public sealed class Partner : Entity
{
    public string Name { get; set; } = string.Empty;
    public PartnerType Type { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? IataCode { get; set; }
    public bool IsActive { get; set; } = true;
}
