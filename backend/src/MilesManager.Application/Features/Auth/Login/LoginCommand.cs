using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthSession>>;
