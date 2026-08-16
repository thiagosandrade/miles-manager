using MilesManager.Domain.Common;

namespace MilesManager.Domain.Users
{
    public sealed class UserRole : Entity
    {
        public Guid UserId { get; set; }

        public AppUser User { get; set; } = null!;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;
    }
}
