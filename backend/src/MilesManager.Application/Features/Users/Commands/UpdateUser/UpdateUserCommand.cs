using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(Guid Id, string Email, string FullName, string? Password, bool IsActive) : IRequest<Result<UserDetailsDTO>>;
