#nullable enable
using System.Collections.Generic;
using MilesManager.Domain.Common;
using MilesManager.Domain.Common.Enums;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Redemptions;
using MilesManager.Domain.Transfers;

namespace MilesManager.Domain.Programs;

public class Program : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProgramType ProgramType { get; set; }
    public string? Website { get; set; }
    public string? Logo { get; set; }
    public decimal ReferencePricePerThousand { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ProgramBalance> ProgramBalances { get; set; } = new List<ProgramBalance>();
    public virtual ICollection<MileagePurchase> MileagePurchases { get; set; } = new List<MileagePurchase>();
    public virtual ICollection<TransferRoute> OriginTransfers { get; set; } = new List<TransferRoute>();
    public virtual ICollection<TransferRoute> DestinationTransfers { get; set; } = new List<TransferRoute>();
    public virtual ICollection<FlightRedemption> FlightRedemptions { get; set; } = new List<FlightRedemption>();
    public virtual ICollection<ReferenceMileagePrice> ReferenceMileagePrices { get; set; } = new List<ReferenceMileagePrice>();
}
