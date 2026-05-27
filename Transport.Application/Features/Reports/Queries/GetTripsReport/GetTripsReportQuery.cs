using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetTripsReport
{
    public record GetTripsReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<TripReportDto>>, IDateRangeReportQuery;
}
