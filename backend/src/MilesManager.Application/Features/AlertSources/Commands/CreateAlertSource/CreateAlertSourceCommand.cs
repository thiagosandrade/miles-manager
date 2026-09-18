using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.AlertSources.Commands.CreateAlertSource;

public sealed record CreateAlertSourceCommand(string Title, string ExternalUrl, string Country) : IRequest<Result<Guid>>;
