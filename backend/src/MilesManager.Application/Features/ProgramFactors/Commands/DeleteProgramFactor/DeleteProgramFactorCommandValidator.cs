using FluentValidation;

namespace MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;

public sealed class DeleteProgramFactorCommandValidator : AbstractValidator<DeleteProgramFactorCommand>
{
    public DeleteProgramFactorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
