using MediatR;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Commands.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<OperationStatus>;
