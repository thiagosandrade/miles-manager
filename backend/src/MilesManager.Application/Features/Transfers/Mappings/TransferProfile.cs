#nullable enable
using AutoMapper;
using MilesManager.Application.Features.Transfers.DTOs;
using MilesManager.Domain.Transfers;

namespace MilesManager.Application.Features.Transfers.Mappings;

internal sealed class TransferProfile : Profile
{
    public TransferProfile()
    {
        CreateMap<MileageTransfer, MileageTransferDto>()
            .ForMember(dest => dest.OriginProgram, opt => opt.MapFrom(src => src.OriginProgram.Name))
            .ForMember(dest => dest.DestinationProgram, opt => opt.MapFrom(src => src.DestinationProgram.Name));
        
    }
}
