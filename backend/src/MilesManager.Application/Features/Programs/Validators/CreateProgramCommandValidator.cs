#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Programs.Commands.CreateProgram;

namespace MilesManager.Application.Features.Programs.Validators;

public sealed class CreateProgramCommandValidator : AbstractValidator<CreateProgramCommand>
{
    public CreateProgramCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Program name is required.");
        RuleFor(x => x.ProgramCode).NotEmpty().WithMessage("Program code is required.");
        RuleFor(x => x.ReferencePricePerThousand).GreaterThanOrEqualTo(0);
    }
}
