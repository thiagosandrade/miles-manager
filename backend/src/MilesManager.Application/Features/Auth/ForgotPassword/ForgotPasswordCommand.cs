using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
