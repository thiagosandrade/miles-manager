using MilesManager.Domain.Common;

namespace MilesManager.Domain.Users;

public sealed class AppUser : Entity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int TokenVersion { get; set; }
    public DateTimeOffset? LastLoginAtUtc { get; set; }
    public ICollection<UserRole> UserRoles { get; } = [];
}
