#nullable enable
using MediatR;
using MilesManager.Application.Features.Lookup.DTOs;

namespace MilesManager.Application.Features.Lookup.Queries.GetLookup;

public sealed class GetLookupQuery : IRequest<LookupDto>
{
}
