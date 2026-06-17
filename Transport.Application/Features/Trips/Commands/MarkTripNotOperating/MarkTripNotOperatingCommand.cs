using MediatR;

namespace Transport.Application.Features.Trips.Commands.MarkTripNotOperating
{
    public class MarkTripNotOperatingCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
