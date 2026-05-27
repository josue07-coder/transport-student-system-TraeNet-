using MediatR;
using Transport.Application.Features.Incidents.DTOs;

namespace Transport.Application.Features.Incidents.Queries.GetMyReportedIncidents
{
    public class GetMyReportedIncidentsQuery : IRequest<List<IncidentResponseDto>>
    {
    }
}
