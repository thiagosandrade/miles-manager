using AutoMapper;
using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDTO>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IReadOnlyList<UserDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAsync(cancellationToken);

        var usersDTO = mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserDTO>>(users);

        return usersDTO;
    }
}
