using MediatR;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest<OperationStatus>;
