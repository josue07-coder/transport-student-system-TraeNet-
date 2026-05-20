using MediatR;

namespace Transport.Application.Features.Trips.Commands.CancelTrip
{
    public class CancelTripCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
