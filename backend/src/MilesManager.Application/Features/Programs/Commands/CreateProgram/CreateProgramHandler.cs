using MediatR;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using Microsoft.EntityFrameworkCore;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Programs;

namespace MilesManager.Application.Features.Programs.Commands.CreateProgram;

public sealed class CreateProgramHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProgramCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Programs
            .AsNoTracking()
            .AnyAsync(
                p => p.ProgramCode == request.ProgramCode,
                cancellationToken);

        if (exists)
        {
            return Result<Guid>.Conflict("Program code already exists.");
        }

        var program = new LoyaltyProgram
        {
            Name = request.Name?.Trim() ?? string.Empty,
            ProgramCode = request.ProgramCode?.Trim() ?? string.Empty,
            ProgramType = request.ProgramType,
            Website = request.Website,
            Logo = request.Logo,
            IsActive = request.IsActive,
            CurrencyCode = request.CurrencyCode
        };

        await context.Programs.AddAsync(program, cancellationToken);

        var referencePrice = new ReferenceMileagePrice
        {
            Program = program,
            PricePerThousand = request.ReferencePricePerThousand,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await context.ReferenceMileagePrices.AddAsync(referencePrice, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(program.Id);
    }
}
