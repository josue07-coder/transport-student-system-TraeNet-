using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByDriver
{
    public record GetRouteAssignmentsByDriverQuery(Guid DriverId) : IRequest<List<RouteAssignmentResponseDto>>;
}
