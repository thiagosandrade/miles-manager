#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Purchases.DTOs;

namespace MilesManager.Application.Features.Purchases.Queries.GetPurchases;

public sealed record GetPurchasesQuery()
    : IRequest<Result<IReadOnlyList<MileagePurchaseDto>>>;
