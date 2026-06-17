using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;

namespace Transport.Application.Features.TripSchedules.Queries.GetActiveTripSchedulesByAssignment
{
    public class GetActiveTripSchedulesByAssignmentQuery : IRequest<List<TripScheduleResponseDto>>
    {
        public Guid RouteAssignmentId { get; }

        public GetActiveTripSchedulesByAssignmentQuery(Guid routeAssignmentId)
        {
            RouteAssignmentId = routeAssignmentId;
        }
    }
}
