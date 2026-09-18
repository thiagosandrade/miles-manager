using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Alerts.DTOs;

namespace MilesManager.Application.Features.Alerts.Queries.GetAlertItems;

public sealed record GetAlertItemsQuery : IRequest<Result<IReadOnlyList<AlertItemDto>>>;
