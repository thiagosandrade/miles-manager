using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Transfers.Queries.GetTransfers;

public sealed class GetTransfersHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetTransfersQuery, Result<IReadOnlyList<DTOs.MileageTransferDto>>>
{
    public async Task<Result<IReadOnlyList<DTOs.MileageTransferDto>>> Handle(GetTransfersQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var items = await context.MileageTransfers
            .Where(x => x.UserId ==  loggedUserId)
            .AsNoTracking()
            .ProjectTo<DTOs.MileageTransferDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DTOs.MileageTransferDto>>.Success(items);
    }
}
