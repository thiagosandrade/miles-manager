using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Email, string FullName, string Password) : IRequest<Result<UserDetailsDTO>>;
