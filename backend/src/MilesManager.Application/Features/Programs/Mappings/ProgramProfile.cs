#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Programs.DTOs;
using MilesManager.Domain.Programs;

namespace MilesManager.Application.Features.Programs.Mappings;

internal sealed class ProgramProfile : Profile
{
    public ProgramProfile()
    {
        CreateMap<LoyaltyProgram, ProgramDto>()
            .ForMember(
                dest => dest.ReferencePricePerThousand,
                opt => opt.MapFrom(src =>
                    src.ReferenceMileagePrices
                        .Where(x =>
                            x.EffectiveDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                        .OrderByDescending(x => x.EffectiveDate)
                        .Select(x => x.PricePerThousand)
                        .FirstOrDefault()
                ));

        CreateMap<ProgramBalance, ProgramBalanceDto>()
            .ForMember(
                dest => dest.ReferencePricePerThousand,
                opt => opt.MapFrom(src =>
                    src.Program.ReferenceMileagePrices
                        .Where(x =>
                            x.EffectiveDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                        .OrderByDescending(x => x.EffectiveDate)
                        .Select(x => x.PricePerThousand)
                        .FirstOrDefault()
                ))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.Name));
    }
}
