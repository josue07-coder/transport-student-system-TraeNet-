using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetLowPresenceReport
{
    public record GetLowPresenceReportQuery(
        DateTime StartDate,
        DateTime EndDate,
        decimal MaximumPresencePercentage = 80) : IRequest<List<LowPresenceStudentReportDto>>, IDateRangeReportQuery;
}
