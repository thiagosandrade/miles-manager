using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed record ValidatePasswordResetTokenQuery(string Token) : IRequest<Result<PasswordResetTokenValidation>>;

public sealed record PasswordResetTokenValidation(bool Valid, string? Email);
