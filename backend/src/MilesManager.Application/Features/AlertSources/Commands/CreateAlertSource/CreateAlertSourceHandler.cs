using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Domain.Alerts;

namespace MilesManager.Application.Features.AlertSources.Commands.CreateAlertSource;

public sealed class CreateAlertSourceHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAlertSourceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateAlertSourceCommand request,
        CancellationToken cancellationToken)
    {
        var title = request.Title.Trim();
        var externalUrl = request.ExternalUrl.Trim();
        var country = request.Country.Trim();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(externalUrl) || string.IsNullOrWhiteSpace(country))
        {
            return Result<Guid>.ValidationFailed("Title, external URL and country are required.");
        }

        if (!Uri.TryCreate(externalUrl, UriKind.Absolute, out _))
        {
            return Result<Guid>.ValidationFailed("External URL must be a valid absolute URL.");
        }

        var exists = await context.AlertSources
            .AsNoTracking()
            .AnyAsync(x => x.ExternalUrl == externalUrl, cancellationToken);

        if (exists)
        {
            return Result<Guid>.Conflict("An alert source with this URL already exists.");
        }

        var source = new AlertSource
        {
            Title = title,
            ExternalUrl = externalUrl,
            Country = country
        };

        await context.AlertSources.AddAsync(source, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(source.Id);
    }
}
