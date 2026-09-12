using MediatR;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramDetails.DTOs;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.ProgramDetails.Queries.GetProgramDetails;

public sealed class GetProgramDetailsHandler(IApplicationDbContext context)
    : IRequestHandler<GetProgramDetailsQuery, Result<ProgramDetailsDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ProgramDetailsDto>> Handle(
        GetProgramDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var program = await _context.Programs
            .AsNoTracking()
            .Where(x => x.Id == request.ProgramId)
            .Select(x => new { x.Id, x.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (program is null)
            return Result<ProgramDetailsDto>.NotFound();

        var currentBalance = await _context.ProgramBalances
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.ProgramId == request.ProgramId)
            .Select(x => (decimal?)x.Balance)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;

        var purchases = await _context.MileagePurchases
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId
                        && x.ProgramId == request.ProgramId
                        && x.PurchaseStatus == PurchaseStatus.Confirmed)
            .Select(x => new
            {
                x.Id,
                Date = x.PurchaseDate,
                x.Description,
                x.Partner,
                Miles = x.MilesPurchased,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var transfers = await _context.MileageTransfers
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId
                        && (x.OriginProgramId == request.ProgramId || x.DestinationProgramId == request.ProgramId))
            .Select(x => new
            {
                x.Id,
                Date = x.TransferDate,
                x.OriginProgramId,
                x.DestinationProgramId,
                x.MilesSent,
                x.MilesReceived,
                OriginProgramName = x.OriginProgram.Name,
                DestinationProgramName = x.DestinationProgram.Name,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var redemptions = await _context.FlightRedemptions
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.ProgramId == request.ProgramId)
            .Select(x => new
            {
                x.Id,
                x.FlightDate,
                x.OriginAirportCode,
                x.DestinationAirportCode,
                x.MilesUsed,
                x.Passengers,
                x.IsRoundTrip,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var events = new List<ActivityEvent>();

        events.AddRange(purchases.Select(x => new ActivityEvent(
            x.Id,
            x.Date,
            x.CreatedAtUtc,
            string.IsNullOrWhiteSpace(x.Description)
                ? $"Purchase{(string.IsNullOrWhiteSpace(x.Partner) ? string.Empty : $" from {x.Partner}")}"
                : x.Description,
            "earned",
            x.Miles)));

        events.AddRange(transfers.Select(x =>
        {
            var isDestination = x.DestinationProgramId == request.ProgramId;
            return new ActivityEvent(
                x.Id,
                x.Date,
                x.CreatedAtUtc,
                isDestination
                    ? $"Transfer from {x.OriginProgramName}"
                    : $"Transfer to {x.DestinationProgramName}",
                isDestination ? "earned" : "redeemed",
                isDestination ? x.MilesReceived : -x.MilesSent);
        }));

        events.AddRange(redemptions.Select(x => new ActivityEvent(
            x.Id,
            x.FlightDate,
            x.CreatedAtUtc,
            $"Redemption - {x.OriginAirportCode} → {x.DestinationAirportCode}",
            "redeemed",
            -x.MilesUsed)));

        var activity = new List<ProgramActivityDto>(events.Count);
        var runningBalance = currentBalance;

        foreach (var item in events
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id))
        {
            activity.Add(new ProgramActivityDto
            {
                Date = item.Date,
                Description = item.Description,
                Type = item.Type,
                Miles = item.Miles,
                Balance = runningBalance
            });

            runningBalance -= item.Miles;
        }

        var redemptionItems = redemptions
            .OrderByDescending(x => x.FlightDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new ProgramRedemptionDto
            {
                Id = x.Id,
                FlightDate = x.FlightDate,
                OriginAirportCode = x.OriginAirportCode,
                DestinationAirportCode = x.DestinationAirportCode,
                MilesUsed = x.MilesUsed,
                Passengers = x.Passengers,
                IsRoundTrip = x.IsRoundTrip
            })
            .ToList();

        return Result<ProgramDetailsDto>.Success(new ProgramDetailsDto
        {
            ProgramId = program.Id,
            ProgramName = program.Name,
            Activity = activity,
            Redemptions = redemptionItems
        });
    }

    private sealed record ActivityEvent(
        Guid Id,
        DateOnly Date,
        DateTimeOffset CreatedAtUtc,
        string Description,
        string Type,
        decimal Miles);
}
