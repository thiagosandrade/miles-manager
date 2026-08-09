using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MilesManager.Application.Features.Transfers.Queries.GetTransfers;

public sealed class GetTransfersHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetTransfersQuery, Result<IReadOnlyList<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IReadOnlyList<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>>> Handle(GetTransfersQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.MileageTransfers.AsNoTracking()
            .ProjectTo<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>>.Success(items);
    }
}
