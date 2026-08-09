#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;

namespace MilesManager.Application.Features.Purchases.Validators;

public sealed class UpdateMileagePurchaseCommandValidator : AbstractValidator<UpdateMileagePurchaseCommand>
{
    public UpdateMileagePurchaseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProgramId).NotEmpty();
        RuleFor(x => x.AmountPaid).GreaterThan(0).WithMessage("AmountPaid must be greater than 0.");
        RuleFor(x => x.MilesPurchased).GreaterThan(0).WithMessage("MilesPurchased must be greater than 0.");
        RuleFor(x => x.PurchaseDate).NotEmpty();
    }
}
