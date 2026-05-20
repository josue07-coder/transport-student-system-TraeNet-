using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Trips.Queries.GetActiveTripByRouteAssignment
{
    public record GetActiveTripByRouteAssignmentQuery(Guid RouteAssignmentId) : IRequest<TripDetailDto>;
}
