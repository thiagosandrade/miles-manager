namespace MilesManager.Application.Features.Alerts.DTOs;

public sealed record AlertItemDto(
    Guid Id,
    string Title,
    DateTimeOffset? PublishedDate,
    string? Chips,
    string ExternalUrl,
    string Country,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
