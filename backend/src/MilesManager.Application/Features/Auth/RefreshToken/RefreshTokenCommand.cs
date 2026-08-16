using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(Guid UserId) : IRequest<Result<AuthSession>>;
