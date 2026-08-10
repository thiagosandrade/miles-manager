using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Programs.Commands.UpdateProgram;

public sealed class UpdateProgramHandler(IApplicationDbContext context) : IRequestHandler<UpdateProgramCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.Programs.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (program is null)
        {
            return Result<Guid>.NotFound();
        }

        program.Name = request.Name?.Trim() ?? program.Name;
        program.ProgramCode = request.ProgramCode?.Trim() ?? program.ProgramCode;
        program.ProgramType = request.ProgramType;
        program.Website = request.Website;
        program.Logo = request.Logo;
        program.ReferencePricePerThousand = request.ReferencePricePerThousand;
        program.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(program.Id);
    }
}
