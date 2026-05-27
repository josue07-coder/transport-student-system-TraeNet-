using MediatR;
using Transport.Application.Features.Reports.DTOs;
using Transport.Application.Features.Reports.Validators;

namespace Transport.Application.Features.Reports.Queries.GetVehiclesUsageReport
{
    public record GetVehiclesUsageReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<VehicleUsageReportDto>>, IDateRangeReportQuery;
}
