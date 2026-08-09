#nullable enable
using MediatR;
using MilesManager.Application.Features.Dashboard.DTOs;

namespace MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardQuery : IRequest<DashboardDto>
{
}
