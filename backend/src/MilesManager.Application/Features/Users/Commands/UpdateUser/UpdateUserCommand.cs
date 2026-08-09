using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(Guid Id, string Email, string FullName, string? Password, bool IsActive) : IRequest<Result<UserDetails>>;
