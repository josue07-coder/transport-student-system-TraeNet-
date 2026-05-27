using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetDriversPerformanceReport
{
    public record GetDriversPerformanceReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<DriverPerformanceReportDto>>, IDateRangeReportQuery;
}
