using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Domain.Pricing;

namespace MilesManager.Application.Features.Programs.Commands.UpdateProgram
{
    public sealed class UpdateProgramHandler(IApplicationDbContext context) : IRequestHandler<UpdateProgramCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            UpdateProgramCommand request,
            CancellationToken cancellationToken)
        {
            var program = await context.Programs
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (program is null)
            {
                return Result<Guid>.NotFound();
            }

            program.Name = request.Name?.Trim() ?? program.Name;
            program.ProgramCode = request.ProgramCode?.Trim() ?? program.ProgramCode;
            program.ProgramType = request.ProgramType;
            program.Website = request.Website;
            program.Logo = request.Logo;
            program.IsActive = request.IsActive;
            program.CurrencyCode = request.CurrencyCode;

            var latestPrice = await context.ReferenceMileagePrices
                .Where(x => x.ProgramId == program.Id)
                .OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestPrice is null ||
                latestPrice.PricePerThousand != request.ReferencePricePerThousand)
            {
                context.ReferenceMileagePrices.Add(
                    new ReferenceMileagePrice
                    {
                        ProgramId = program.Id,
                        PricePerThousand = request.ReferencePricePerThousand,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    });
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(program.Id);
        }
    }
}
