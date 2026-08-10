using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Transfers;

namespace MilesManager.Domain.Programs;

public class LoyaltyProgram : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Logo { get; set; }
    public ProgramType ProgramType { get; set; }
    public decimal ReferencePricePerThousand { get; set; }
    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }
    public Guid PartnerId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public ProgramStatus Status { get; set; } = ProgramStatus.Active;
    public int ExpirationAfterMonths { get; set; } = 36;
    public bool ExpirationEnabled { get; set; } = true;

    public virtual ICollection<ProgramBalance> ProgramBalances { get; set; } = new List<ProgramBalance>();
    public virtual ICollection<MileagePurchase> MileagePurchases { get; set; } = new List<MileagePurchase>();
    public virtual ICollection<FlightRedemption> FlightRedemptions { get; set; } = new List<FlightRedemption>();
    public virtual ICollection<ReferenceMileagePrice> ReferenceMileagePrices { get; set; } = new List<ReferenceMileagePrice>();
    public virtual ICollection<TransferRoute> OriginTransfers { get; set; } = new List<TransferRoute>();
    public virtual ICollection<TransferRoute> DestinationTransfers { get; set; } = new List<TransferRoute>();
}
