using MilesManager.Domain.Common;

namespace MilesManager.Domain.Opportunities;

public sealed class OpportunitySource : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public OpportunitySourceType SourceType { get; set; } = OpportunitySourceType.WebPage;
    public bool IsEnabled { get; set; } = true;
    public string? Configuration { get; set; }
    public DateTime? LastIngestionAtUtc { get; set; }
    public DateTime? LastSuccessfulIngestionAtUtc { get; set; }
    public string? LastError { get; set; }
    public int OpportunitiesDiscovered { get; set; }

    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
