using MediatR;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.AlertSources.Commands.DeleteAlertSource;

public sealed class DeleteAlertSourceHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteAlertSourceCommand, Result>
{
    public async Task<Result> Handle(
        DeleteAlertSourceCommand request,
        CancellationToken cancellationToken)
    {
        var source = await context.AlertSources
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (source is null)
        {
            return Result.NotFound();
        }

        context.AlertSources.Remove(source);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
