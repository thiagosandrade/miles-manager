using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Alerts.DTOs;

namespace MilesManager.Application.Features.Alerts.Queries.GetAlertItems;

public sealed class GetAlertItemsHandler(IApplicationDbContext context)
    : IRequestHandler<GetAlertItemsQuery, Result<IReadOnlyList<AlertItemDto>>>
{
    public async Task<Result<IReadOnlyList<AlertItemDto>>> Handle(
        GetAlertItemsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await context.AlertItems
            .AsNoTracking()
            .OrderByDescending(x => x.PublishedDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new AlertItemDto(
                x.Id,
                x.Title,
                x.PublishedDate,
                x.Chips,
                x.ExternalUrl,
                x.Country,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AlertItemDto>>.Success(items);
    }
}
