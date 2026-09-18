using AutoMapper;
using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDetailsDTO>>
{
    public async Task<IReadOnlyList<UserDetailsDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.ListAsync(cancellationToken);
        var usersDto = mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserDetailsDTO>>(users);
        return usersDto;
    }
}
