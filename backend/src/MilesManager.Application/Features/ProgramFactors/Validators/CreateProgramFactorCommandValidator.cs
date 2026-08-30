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
        RuleFor(x => x.TransferFactor)
            .GreaterThan(0)
            .WithMessage("TransferFactor must be greater than 0.");
    }
}
