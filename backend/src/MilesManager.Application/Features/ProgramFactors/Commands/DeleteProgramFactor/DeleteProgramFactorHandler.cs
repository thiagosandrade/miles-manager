using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;

public sealed class DeleteProgramFactorHandler(IApplicationDbContext context) : IRequestHandler<DeleteProgramFactorCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(DeleteProgramFactorCommand request, CancellationToken cancellationToken)
    {
        var transferRoute = await _context.TransferRoutes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transferRoute is null)
        {
            return Result.NotFound();
        }

        _context.TransferRoutes.Remove(transferRoute);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
