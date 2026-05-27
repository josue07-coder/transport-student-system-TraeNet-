using MediatR;
using Transport.Application.Features.Reports.DTOs;

namespace Transport.Application.Features.Reports.Queries.GetStudentsByRouteReport
{
    public record GetStudentsByRouteReportQuery(Guid RouteId) : IRequest<StudentsByRouteReportDto>;
}
