using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Alerts.Commands.DeleteAlertItem;

public sealed class DeleteAlertItemHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteAlertItemCommand, Result>
{
    public async Task<Result> Handle(
        DeleteAlertItemCommand request,
        CancellationToken cancellationToken)
    {
        var alert = await context.AlertItems
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (alert is null)
        {
            return Result.NotFound();
        }

        context.AlertItems.Remove(alert);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
