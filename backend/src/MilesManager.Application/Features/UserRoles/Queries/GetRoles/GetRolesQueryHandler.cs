using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.UserRoles.DTOs;

namespace MilesManager.Application.Features.UserRoles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper) : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDTO>>
{
    public async Task<IReadOnlyList<RoleDTO>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await applicationDbContext
            .Roles
            .AsNoTracking()
            .ProjectTo<RoleDTO>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);

        return roles;
    }
}
