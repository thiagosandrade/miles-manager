using MediatR;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Auth.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<OperationStatus>;
