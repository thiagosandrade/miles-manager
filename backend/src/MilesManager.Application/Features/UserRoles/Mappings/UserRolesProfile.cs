#nullable enable
using AutoMapper;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.UserRoles.Mappings;

internal sealed class UserRolesProfile : Profile
{
    public UserRolesProfile()
    {
        CreateMap<UserRole, UserRoleDTO>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        CreateMap<Role, RoleDTO>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id));
            
    }
}
