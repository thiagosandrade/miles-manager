using MediatR;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Lookup.DTOs;

namespace MilesManager.Application.Features.Lookup.Queries.GetLookup;

public sealed class GetLookupQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetLookupQuery, LookupDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<LookupDto> Handle(GetLookupQuery request, CancellationToken cancellationToken)
    {
        var airports = await _context.Airports.AsNoTracking()
            .OrderBy(x => x.City)
            .ProjectTo<AirportDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var dto = new LookupDto
        {
            Airports = airports
        };

        return dto;
    }
}
