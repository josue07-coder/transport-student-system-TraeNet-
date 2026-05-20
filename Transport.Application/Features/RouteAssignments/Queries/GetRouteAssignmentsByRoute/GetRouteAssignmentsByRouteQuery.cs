using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByRoute
{
    public record GetRouteAssignmentsByRouteQuery(Guid RouteId) : IRequest<List<RouteAssignmentResponseDto>>;
}
