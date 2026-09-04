using FluentValidation;
using MilesManager.Application.Features.Auth.ForgotPassword.Commands;

namespace MilesManager.Application.Features.Auth.ForgotPassword.Validators;

internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}
