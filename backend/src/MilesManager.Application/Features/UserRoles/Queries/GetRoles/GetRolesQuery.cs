using MediatR;
using MilesManager.Application.Features.UserRoles.DTOs;

namespace MilesManager.Application.Features.UserRoles.Queries.GetRoles;

public sealed record GetRolesQuery() : IRequest<IReadOnlyList<RoleDTO>>;
