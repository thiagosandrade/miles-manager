using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;

public sealed class GetMileageTransferByIdHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetMileageTransferByIdQuery, Result<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>> Handle(GetMileageTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.MileageTransfers.AsNoTracking()
            .Where(t => t.Id == request.Id)
            .ProjectTo<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>.NotFound();
        }

        return Result<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>.Success(item);
    }
}
