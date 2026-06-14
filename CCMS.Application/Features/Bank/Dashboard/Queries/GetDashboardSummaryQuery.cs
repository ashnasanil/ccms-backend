using MediatR;
using CCMS.Application.Features.Bank.Dashboard.DTOs;

namespace CCMS.Application.Features.Bank.Dashboard.Queries
{
    public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;
}
