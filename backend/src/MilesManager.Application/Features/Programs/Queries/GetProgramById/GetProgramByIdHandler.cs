using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Programs.Queries.GetProgramById;

public sealed class GetProgramByIdHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetProgramByIdQuery, Result<MilesManager.Application.Features.Programs.DTOs.ProgramDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<DTOs.ProgramDto>> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.Programs.AsNoTracking()
            .Where(p => p.Id == request.Id)
            .ProjectTo<MilesManager.Application.Features.Programs.DTOs.ProgramDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<MilesManager.Application.Features.Programs.DTOs.ProgramDto>.NotFound();
        }

        return Result<MilesManager.Application.Features.Programs.DTOs.ProgramDto>.Success(item);
    }
}
