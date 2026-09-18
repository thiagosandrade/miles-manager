using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.AlertSources.Commands.DeleteAlertSource;

public sealed record DeleteAlertSourceCommand(Guid Id) : IRequest<Result>;
