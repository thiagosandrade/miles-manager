using MilesManager.Domain.Common;

namespace MilesManager.Domain.Opportunities;

public sealed class Opportunity : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public OpportunityType Type { get; set; }
    public string? SourceProgram { get; set; }
    public string? DestinationProgram { get; set; }
    public string? Benefit { get; set; }
    public string? Conditions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ExternalUrl { get; set; }
    public Guid OpportunitySourceId { get; set; }
    public OpportunitySource? OpportunitySource { get; set; }
    public OpportunityStatus Status { get; set; } = OpportunityStatus.Pending;
    public DateTime DiscoveredAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string? OriginalContent { get; set; }
}
