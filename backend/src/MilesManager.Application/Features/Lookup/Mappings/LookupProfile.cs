#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Lookup.DTOs;
using MilesManager.Domain.Lookups;

namespace MilesManager.Application.Features.Lookup.Mappings;

internal sealed class LookupProfile : Profile
{
    public LookupProfile()
    {
        CreateMap<Airport, AirportDto>();
    }
}
