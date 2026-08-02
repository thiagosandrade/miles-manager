using MediatR;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<OperationResult<AuthSession>>;
