using MilesManager.Domain.Common;

namespace MilesManager.Domain.Users
{
    public sealed class Role : Entity
    {
        public string Name { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; } = [];
    }
}
