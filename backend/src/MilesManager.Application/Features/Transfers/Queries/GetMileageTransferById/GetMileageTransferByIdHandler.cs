using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Abstractions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Context;

namespace MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;

public sealed class GetMileageTransferByIdHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetMileageTransferByIdQuery, Result<DTOs.MileageTransferDto>>
{
    public async Task<Result<DTOs.MileageTransferDto>> Handle(GetMileageTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var loggedUserId = currentUser.UserId;

        var item = await context.MileageTransfers
            .AsNoTracking()
            .Where(t => t.Id == request.Id && t.UserId == loggedUserId)
            .ProjectTo<DTOs.MileageTransferDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result<DTOs.MileageTransferDto>.NotFound();
        }

        return Result<DTOs.MileageTransferDto>.Success(item);
    }
}
