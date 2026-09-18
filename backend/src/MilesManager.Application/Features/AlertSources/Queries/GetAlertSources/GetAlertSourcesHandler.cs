using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.AlertSources.DTOs;

namespace MilesManager.Application.Features.AlertSources.Queries.GetAlertSources;

public sealed class GetAlertSourcesHandler(IApplicationDbContext context)
    : IRequestHandler<GetAlertSourcesQuery, Result<IReadOnlyList<AlertSourceDto>>>
{
    public async Task<Result<IReadOnlyList<AlertSourceDto>>> Handle(
        GetAlertSourcesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await context.AlertSources
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .Select(x => new AlertSourceDto(
                x.Id,
                x.Title,
                x.ExternalUrl,
                x.Country,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AlertSourceDto>>.Success(items);
    }
}
