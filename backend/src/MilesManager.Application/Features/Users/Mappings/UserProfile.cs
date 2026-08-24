#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.UserRoles.Mappings;

internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, UserDetailsDTO>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles));
    }
}
