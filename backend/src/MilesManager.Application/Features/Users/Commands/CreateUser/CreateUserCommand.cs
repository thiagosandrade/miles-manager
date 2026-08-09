using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Email, string FullName, string Password) : IRequest<Result<UserDetails>>;
