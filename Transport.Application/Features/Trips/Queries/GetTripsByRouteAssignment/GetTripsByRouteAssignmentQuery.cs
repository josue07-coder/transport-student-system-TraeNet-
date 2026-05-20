using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Trips.Queries.GetTripsByRouteAssignment
{
    public record GetTripsByRouteAssignmentQuery(Guid RouteAssignmentId) : IRequest<List<TripResponseDto>>;
}
