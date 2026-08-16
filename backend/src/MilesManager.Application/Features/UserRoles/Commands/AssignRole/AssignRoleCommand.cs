using MediatR;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.UserRoles.Commands.AssignRole
{
    public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<OperationStatus>;
}
