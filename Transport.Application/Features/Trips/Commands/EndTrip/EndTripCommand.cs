using MediatR;

namespace Transport.Application.Features.Trips.Commands.EndTrip
{
    public class EndTripCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
