using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

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
        program.ProgramCode = request.Code?.Trim() ?? program.ProgramCode;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(program.Id);
    }
}
