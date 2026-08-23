#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Purchases.DTOs;
using MilesManager.Domain.Programs;

namespace MilesManager.Application.Features.Purchases.Mappings;

internal sealed class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        CreateMap<MileagePurchase, MileagePurchaseDto>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.Name))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Program.CurrencyCode));
    }
}
