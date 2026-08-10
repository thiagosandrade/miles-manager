#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Programs.Commands.UpdateProgram;

namespace MilesManager.Application.Features.Programs.Validators;

public sealed class UpdateProgramCommandValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Program name is required.");
        RuleFor(x => x.ProgramCode).NotEmpty().WithMessage("Program code is required.");
        RuleFor(x => x.ReferencePricePerThousand).GreaterThanOrEqualTo(0);
    }
}
