using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByVehicle
{
    public record GetRouteAssignmentsByVehicleQuery(Guid VehicleId) : IRequest<List<RouteAssignmentResponseDto>>;
}
