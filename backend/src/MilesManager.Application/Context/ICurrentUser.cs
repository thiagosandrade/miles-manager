namespace MilesManager.Application.Context
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }

        string? Email { get; }

        string? Name { get; }

        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}
