using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MilesManager.Application.Context
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor)
        : ICurrentUser
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User?.FindFirstValue("sub");

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Email)
            ?? User?.FindFirstValue("email");

        public string? Name =>
            User?.FindFirstValue(ClaimTypes.Name)
            ?? User?.FindFirstValue("name");

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role) =>
            User?.IsInRole(role) ?? false;
    }
}
