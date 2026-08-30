using FluentValidation;
using MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;

namespace MilesManager.Application.Features.ProgramFactors.Validators;

public sealed class CreateProgramFactorCommandValidator : AbstractValidator<CreateProgramFactorCommand>
{
    public CreateProgramFactorCommandValidator()
    {
        RuleFor(x => x.OriginProgramId).NotEmpty();
        RuleFor(x => x.DestinationProgramId).NotEmpty();
        RuleFor(x => x.OriginProgramId)
            .NotEqual(x => x.DestinationProgramId)
            .WithMessage("Origin and destination programs must be different.");
        RuleFor(x => x.FromAmount)
            .GreaterThan(0)
            .WithMessage("FromAmount must be greater than 0.");
        RuleFor(x => x.ToAmount)
            .GreaterThan(0)
            .WithMessage("ToAmount must be greater than 0.");
    }
}
