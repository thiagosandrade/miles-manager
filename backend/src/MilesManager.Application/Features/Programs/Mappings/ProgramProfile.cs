#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Programs.DTOs;
using MilesManager.Domain.Programs;

namespace MilesManager.Application.Features.Programs.Mappings;

internal sealed class ProgramProfile : Profile
{
    public ProgramProfile()
    {
        CreateMap<LoyaltyProgram, ProgramDto>();
        CreateMap<ProgramBalance, ProgramBalanceDto>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.Name));
    }
}
