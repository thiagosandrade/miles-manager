using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.UserRoles.Commands.AssignRole;

public sealed class AssignRoleHandler(IUserRepository userRepository, IApplicationDbContext context)
    : IRequestHandler<AssignRoleCommand, OperationStatus>
{
    public async Task<OperationStatus> Handle(
        AssignRoleCommand command,
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

        var alreadyAssigned = await context
            .UserRoles
            .AnyAsync(x => x.UserId == command.UserId && x.RoleId == command.RoleId, cancellationToken);

        if (alreadyAssigned)
        {
            return OperationStatus.Conflict;
        }

        var userRole = new UserRole
        {
            UserId = command.UserId,
            RoleId = command.RoleId
        };

        await context.UserRoles.AddAsync(userRole, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return OperationStatus.Success;
    }
}
