using MediatR;

namespace Transport.Application.Features.Trips.Commands.CancelTrip
{
    public class CancelTripCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
