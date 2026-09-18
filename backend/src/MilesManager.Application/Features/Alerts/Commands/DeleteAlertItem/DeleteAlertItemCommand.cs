using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Alerts.Commands.DeleteAlertItem;

public sealed record DeleteAlertItemCommand(Guid Id) : IRequest<Result>;
