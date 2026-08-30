using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Domain.Transfers;

namespace MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;

public sealed class CreateProgramFactorHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProgramFactorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateProgramFactorCommand request,
        CancellationToken cancellationToken)
    {
        var programsExist = await context.Programs
            .AsNoTracking()
            .Where(p => p.Id == request.OriginProgramId || p.Id == request.DestinationProgramId)
            .Select(p => p.Id)
            .CountAsync(cancellationToken);

        if (programsExist != 2)
        {
            return Result<Guid>.NotFound();
        }

        var exists = await context.TransferRoutes
            .AsNoTracking()
            .AnyAsync(
                x => x.OriginProgramId == request.OriginProgramId &&
                     x.DestinationProgramId == request.DestinationProgramId,
                cancellationToken);

        if (exists)
        {
            return Result<Guid>.Conflict("A transfer factor already exists for this program pair.");
        }

        var transferRoute = new TransferRoute
        {
            OriginProgramId = request.OriginProgramId,
            DestinationProgramId = request.DestinationProgramId,
            FromAmount = request.FromAmount,
            ToAmount = request.ToAmount
        };

        await context.TransferRoutes.AddAsync(transferRoute, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transferRoute.Id);
    }
}
