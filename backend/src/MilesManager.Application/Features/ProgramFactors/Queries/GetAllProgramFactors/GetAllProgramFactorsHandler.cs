using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.DTOs;

namespace MilesManager.Application.Features.ProgramFactors.Queries.GetAllProgramFactors;

public sealed class GetAllProgramFactorsHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllProgramFactorsQuery, Result<IReadOnlyList<ProgramFactorDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<IReadOnlyList<ProgramFactorDto>>> Handle(
        GetAllProgramFactorsQuery request,
        CancellationToken cancellationToken)
    {
        var factors = await _context.TransferRoutes
            .AsNoTracking()
            .Include(x => x.OriginProgram)
            .Include(x => x.DestinationProgram)
            .OrderBy(x => x.OriginProgram.Name)
            .ThenBy(x => x.DestinationProgram.Name)
            .Select(x => new ProgramFactorDto
            {
                Id = x.Id,
                OriginProgramId = x.OriginProgramId,
                OriginProgramName = x.OriginProgram.Name,
                DestinationProgramId = x.DestinationProgramId,
                DestinationProgramName = x.DestinationProgram.Name,
                TransferFactor = x.TransferFactor
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ProgramFactorDto>>.Success(factors);
    }
}
