using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetAuditSummaryReport
{
    public record GetAuditSummaryReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<AuditSummaryReportDto>>, IDateRangeReportQuery;
}
