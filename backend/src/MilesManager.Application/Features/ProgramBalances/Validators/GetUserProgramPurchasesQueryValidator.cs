using FluentValidation;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

namespace MilesManager.Application.Features.ProgramBalances.Validators;

public sealed class GetUserProgramPurchasesQueryValidator : AbstractValidator<GetUserProgramPurchasesQuery>
{
    public GetUserProgramPurchasesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id is required.");
    }
}
