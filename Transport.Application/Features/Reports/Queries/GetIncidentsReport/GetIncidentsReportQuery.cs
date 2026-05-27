using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetIncidentsReport
{
    public record GetIncidentsReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<IncidentReportDto>>, IDateRangeReportQuery;
}
