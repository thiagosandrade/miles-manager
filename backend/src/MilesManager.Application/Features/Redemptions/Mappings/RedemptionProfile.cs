#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Redemptions.DTOs;
using MilesManager.Domain.Redemptions;

namespace MilesManager.Application.Features.Redemptions.Mappings;

internal sealed class RedemptionProfile : Profile
{
    public RedemptionProfile()
    {
        CreateMap<FlightRedemption, FlightRedemptionDto>()
            .ForMember(dest => dest.Program, opt => opt.MapFrom(src => src.LoyaltyProgram.Name));
    }
}
