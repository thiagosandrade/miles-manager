using MediatR;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Auth.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<OperationStatus>;
