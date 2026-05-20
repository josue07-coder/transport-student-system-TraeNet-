using MediatR;

namespace Transport.Application.Features.Trips.Commands.StartTrip
{
    public class StartTripCommand : IRequest<Guid>
    {
        public Guid RouteAssignmentId { get; set; }
    }
}
