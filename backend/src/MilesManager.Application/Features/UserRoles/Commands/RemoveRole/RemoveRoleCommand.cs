using MediatR;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.UserRoles.Commands.RemoveRole
{
    public sealed record RemoveRoleCommand(Guid UserId, Guid RoleId) : IRequest<OperationStatus>;
}
