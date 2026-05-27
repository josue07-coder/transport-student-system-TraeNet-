using MediatR;
using Transport.Application.Features.Incidents.DTOs;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByRouteAssignment
{
    public record GetIncidentsByRouteAssignmentQuery(Guid RouteAssignmentId) : IRequest<List<IncidentResponseDto>>;
}
