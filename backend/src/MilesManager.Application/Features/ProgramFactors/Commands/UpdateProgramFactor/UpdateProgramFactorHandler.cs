using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;

public sealed class UpdateProgramFactorHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProgramFactorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        UpdateProgramFactorCommand request,
        CancellationToken cancellationToken)
    {
        var transferRoute = await context.TransferRoutes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transferRoute is null)
        {
            return Result<Guid>.NotFound();
        }

        var programsExist = await context.Programs
            .AsNoTracking()
            .Where(p => p.Id == request.OriginProgramId || p.Id == request.DestinationProgramId)
            .Select(p => p.Id)
            .CountAsync(cancellationToken);

        if (programsExist != 2)
        {
            return Result<Guid>.NotFound();
        }

        var duplicate = await context.TransferRoutes
            .AsNoTracking()
            .AnyAsync(
                x => x.Id != request.Id &&
                     x.OriginProgramId == request.OriginProgramId &&
                     x.DestinationProgramId == request.DestinationProgramId,
                cancellationToken);

        if (duplicate)
        {
            return Result<Guid>.Conflict("A transfer factor already exists for this program pair.");
        }

        transferRoute.OriginProgramId = request.OriginProgramId;
        transferRoute.DestinationProgramId = request.DestinationProgramId;
        transferRoute.TransferFactor = request.TransferFactor;

        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transferRoute.Id);
    }
}
