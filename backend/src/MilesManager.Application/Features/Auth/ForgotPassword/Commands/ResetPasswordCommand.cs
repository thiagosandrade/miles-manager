using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Auth.ForgotPassword.Commands;

public sealed record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
