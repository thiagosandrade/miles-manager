using AutoMapper;
using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDetailsDTO?>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDetailsDTO?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user != null) 
        {
            return mapper.Map<AppUser,UserDetailsDTO>(user);
        }

        return null;
    }
}
