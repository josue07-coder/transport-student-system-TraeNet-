using MediatR;
using Transport.Application.Features.Reports.DTOs;

namespace Transport.Application.Features.Reports.Queries.GetDashboardReport
{
    public record GetDashboardReportQuery : IRequest<DashboardReportDto>;
}
