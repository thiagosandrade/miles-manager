namespace MilesManager.Application.Features.UserRoles.DTOs;

public sealed class UserRoleDTO
{
    public Guid UserId { get; set; } 
    public Guid RoleId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
}

public sealed class RoleDTO
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
}
