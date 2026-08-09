using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Programs.DTOs;

namespace MilesManager.Application.Features.Programs.Queries.GetPrograms;

public sealed class GetProgramsHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetProgramsQuery, Result<IReadOnlyList<ProgramDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IReadOnlyList<ProgramDto>>> Handle(GetProgramsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.Programs.AsNoTracking()
            .ProjectTo<ProgramDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ProgramDto>>.Success(items);
    }
}
