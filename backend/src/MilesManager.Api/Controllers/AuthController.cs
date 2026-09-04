using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Users.Commands.RegisterUser;
using MilesManager.Application.Features.Auth.Login;
using MilesManager.Application.Features.Auth.ForgotPassword;
using MilesManager.Application.Features.Users.DTOs;

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
    public async Task<ActionResult<UserDetailsDTO>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await Sender.Send(command);
        return Convert(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await Sender.Send(command);
        return Convert(result);
    }

    [HttpGet("reset-password/validate")]
    [AllowAnonymous]
    public async Task<ActionResult<PasswordResetTokenValidation>> ValidateResetPasswordToken([FromQuery] string token)
    {
        var result = await Sender.Send(new ValidatePasswordResetTokenQuery(token));
        return result.Status == Application.Common.ResultStatus.Success && result.Value is not null
            ? Ok(result.Value)
            : Convert(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Sender.Send(command);
        return Convert(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthSession>> Refresh()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await Sender.Send(new Application.Features.Auth.RefreshToken.RefreshTokenCommand(userId));
        return Convert(result);
    }
}
