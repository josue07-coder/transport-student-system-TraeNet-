using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetTripPassengers
{
    public class GetTripPassengersQuery : IRequest<List<TripStudentAttendanceDto>>
    {
        public Guid TripId { get; }

        public GetTripPassengersQuery(Guid tripId)
        {
            TripId = tripId;
        }
    }
}
