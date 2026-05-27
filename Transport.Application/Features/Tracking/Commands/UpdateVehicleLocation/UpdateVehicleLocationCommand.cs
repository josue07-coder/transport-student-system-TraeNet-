using MediatR;

namespace Transport.Application.Features.Tracking.Commands.UpdateVehicleLocation
{
    public class UpdateVehicleLocationCommand : IRequest<Guid>
    {
        public Guid TripId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Heading { get; set; }
    }
}
