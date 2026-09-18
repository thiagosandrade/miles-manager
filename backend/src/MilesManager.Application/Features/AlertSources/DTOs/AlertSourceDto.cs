namespace MilesManager.Application.Features.AlertSources.DTOs;

public sealed record AlertSourceDto(
    Guid Id,
    string Title,
    string ExternalUrl,
    string Country,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
