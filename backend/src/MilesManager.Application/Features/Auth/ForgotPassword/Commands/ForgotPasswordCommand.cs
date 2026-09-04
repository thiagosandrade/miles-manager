using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Auth.ForgotPassword.Commands;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
