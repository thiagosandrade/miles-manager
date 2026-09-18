using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.AlertSources.DTOs;

namespace MilesManager.Application.Features.AlertSources.Queries.GetAlertSources;

public sealed record GetAlertSourcesQuery : IRequest<Result<IReadOnlyList<AlertSourceDto>>>;
