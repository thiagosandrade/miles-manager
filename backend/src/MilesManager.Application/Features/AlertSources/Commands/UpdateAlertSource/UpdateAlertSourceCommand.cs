using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.AlertSources.Commands.UpdateAlertSource;

public sealed record UpdateAlertSourceCommand(
    Guid Id,
    string Title,
    string ExternalUrl,
    string Country) : IRequest<Result>;
