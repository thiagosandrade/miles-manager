using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Domain.Alerts;

namespace MilesManager.Application.Features.AlertSources.Commands.UpdateAlertSource;

public sealed class UpdateAlertSourceHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateAlertSourceCommand, Result>
{
    public async Task<Result> Handle(
        UpdateAlertSourceCommand request,
        CancellationToken cancellationToken)
    {
        var title = request.Title.Trim();
        var externalUrl = request.ExternalUrl.Trim();
        var country = request.Country.Trim();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(externalUrl) || string.IsNullOrWhiteSpace(country))
        {
            return Result.ValidationFailed("Title, external URL and country are required.");
        }

        if (!Uri.TryCreate(externalUrl, UriKind.Absolute, out _))
        {
            return Result.ValidationFailed("External URL must be a valid absolute URL.");
        }

        var source = await context.AlertSources
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (source is null)
        {
            return Result.NotFound();
        }

        var duplicate = await context.AlertSources
            .AsNoTracking()
            .AnyAsync(x => x.Id != request.Id && x.ExternalUrl == externalUrl, cancellationToken);

        if (duplicate)
        {
            return Result.Conflict("An alert source with this URL already exists.");
        }

        source.Title = title;
        source.ExternalUrl = externalUrl;
        source.Country = country;
        source.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
