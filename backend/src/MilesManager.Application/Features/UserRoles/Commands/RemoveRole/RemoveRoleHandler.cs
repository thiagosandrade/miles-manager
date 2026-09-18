using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.UserRoles.Commands.RemoveRole;

public sealed class RemoveRoleHandler(IUserRepository userRepository, IApplicationDbContext context)
    : IRequestHandler<RemoveRoleCommand, OperationStatus>
{
    public async Task<OperationStatus> Handle(
        RemoveRoleCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return OperationStatus.NotFound;
        }

        Role? role = await context
            .Roles
            .Where(x => x.Id == command.RoleId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (role is null)
        {
            return OperationStatus.NotFound;
        }

        var notAssigned = !await context
            .UserRoles
            .AnyAsync(x => x.UserId == command.UserId && x.RoleId == command.RoleId, cancellationToken);

        if (notAssigned)
        {
            return OperationStatus.NotFound;
        }

        var userRole = user.UserRoles.First(x => x.RoleId == command.RoleId);

        context.UserRoles.Remove(userRole);

        await context.SaveChangesAsync(cancellationToken);

        return OperationStatus.Success;
    }
}
