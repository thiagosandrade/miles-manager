#nullable enable
using FluentValidation;
using MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;

namespace MilesManager.Application.Features.Purchases.Validators;

public sealed class CreateMileagePurchaseCommandValidator : AbstractValidator<CreateMileagePurchaseCommand>
{
    public CreateMileagePurchaseCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProgramId).NotEmpty();
        RuleFor(x => x.AmountPaid).GreaterThanOrEqualTo(0).WithMessage("AmountPaid must be greater than 0.");
        RuleFor(x => x.MilesPurchased).GreaterThan(0).WithMessage("MilesPurchased must be greater than 0.");
        RuleFor(x => x.PurchaseDate).NotEmpty();
    }
}
