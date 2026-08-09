using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Users.Commands.CreateUser;
using MilesManager.Application.Features.Users.Models;
using MilesManager.Application.Features.Auth.Login;

namespace MilesManager.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public sealed class AuthController(ISender sender) : ApiController(sender)
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthSession>> Login([FromBody] LoginCommand command)
    {
        var result = await Sender.Send(command);
        return Convert(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDetails>> Register([FromBody] CreateUserCommand command)
    {
        var result = await Sender.Send(command);
        return Convert(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthSession>> Refresh()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await Sender.Send(new MilesManager.Application.Features.Auth.RefreshToken.RefreshTokenCommand(userId));
        return Convert(result);
    }
}
