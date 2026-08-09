using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Programs.Commands.DeleteProgram;

public sealed class DeleteProgramHandler(IApplicationDbContext context) : IRequestHandler<DeleteProgramCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.Programs.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (program is null)
        {
            return Result.NotFound();
        }

        _context.Programs.Remove(program);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
