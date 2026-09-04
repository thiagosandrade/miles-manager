using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
