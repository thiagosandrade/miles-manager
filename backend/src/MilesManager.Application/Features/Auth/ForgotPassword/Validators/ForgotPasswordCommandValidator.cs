using FluentValidation;
using MilesManager.Application.Features.Auth.ForgotPassword.Commands;

namespace MilesManager.Application.Features.Auth.ForgotPassword.Validators;

internal sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
