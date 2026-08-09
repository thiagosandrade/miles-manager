#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;

namespace MilesManager.Application.Features.Transfers.Validators;

public sealed class DeleteMileageTransferCommandValidator : AbstractValidator<DeleteMileageTransferCommand>
{
    public DeleteMileageTransferCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
