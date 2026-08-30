using FluentValidation;
using MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;

namespace MilesManager.Application.Features.ProgramFactors.Validators;

public sealed class UpdateProgramFactorCommandValidator : AbstractValidator<UpdateProgramFactorCommand>
{
    public UpdateProgramFactorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
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
