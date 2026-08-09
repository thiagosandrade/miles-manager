#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;

namespace MilesManager.Application.Features.Transfers.Validators;

public sealed class UpdateMileageTransferCommandValidator : AbstractValidator<UpdateMileageTransferCommand>
{
    public UpdateMileageTransferCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OriginProgramId).NotEmpty();
        RuleFor(x => x.DestinationProgramId).NotEmpty();
        RuleFor(x => x.MilesSent).GreaterThan(0).WithMessage("MilesSent must be greater than 0.");
        RuleFor(x => x.TransferDate).NotEmpty().WithMessage("TransferDate is required.");
    }
}
