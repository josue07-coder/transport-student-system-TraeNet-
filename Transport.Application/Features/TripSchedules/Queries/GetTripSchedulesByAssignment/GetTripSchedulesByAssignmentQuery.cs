using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;

namespace Transport.Application.Features.TripSchedules.Queries.GetTripSchedulesByAssignment
{
    public class GetTripSchedulesByAssignmentQuery : IRequest<List<TripScheduleResponseDto>>
    {
        public Guid RouteAssignmentId { get; }

        public GetTripSchedulesByAssignmentQuery(Guid routeAssignmentId)
        {
            RouteAssignmentId = routeAssignmentId;
        }
    }
}
