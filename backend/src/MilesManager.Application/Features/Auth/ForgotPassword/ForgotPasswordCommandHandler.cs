using MediatR;
using Microsoft.Extensions.Configuration;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Email;
using MilesManager.Application.Features.Users.Abstractions;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenService tokenService,
    IEmailService emailService,
    EmailTemplateService emailTemplateService,
    IConfiguration configuration) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(30);

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        // Always return success to avoid revealing whether an account exists or is active.
        if (user is null || !user.IsActive)
        {
            return Result.Success();
        }

        var token = tokenService.CreateToken(user, DateTimeOffset.UtcNow.Add(TokenLifetime));
        var frontendBaseUrl = configuration["Frontend:BaseUrl"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("Missing Frontend:BaseUrl configuration.");
        var resetUrl = $"{frontendBaseUrl}/auth/reset-password?token={Uri.EscapeDataString(token)}";

        var body = await emailTemplateService.RenderAsync(
            "ForgotPassword",
            new Dictionary<string, string>
            {
                ["FullName"] = user.FullName,
                ["ResetUrl"] = resetUrl
            },
            cancellationToken);

        await emailService.SendAsync(
            user.Email,
            "Reset your Miles Manager password",
            body,
            cancellationToken);

        return Result.Success();
    }
}
