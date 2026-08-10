using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Programs.Commands.CreateProgram;

public sealed class CreateProgramHandler(IApplicationDbContext context) : IRequestHandler<CreateProgramCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Programs.AsNoTracking().FirstOrDefaultAsync(p => p.ProgramCode == request.ProgramCode, cancellationToken);
        if (exists is not null)
        {
            return Result<Guid>.Conflict("Program code already exists.");
        }

        var program = new MilesManager.Domain.Programs.LoyaltyProgram
        {
            Name = request.Name?.Trim() ?? string.Empty,
            ProgramCode = request.ProgramCode?.Trim() ?? string.Empty,
            ProgramType = request.ProgramType,
            Website = request.Website,
            Logo = request.Logo,
            ReferencePricePerThousand = request.ReferencePricePerThousand
        };

        await _context.Programs.AddAsync(program, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(program.Id);
    }
}
