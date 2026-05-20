using MediatR;

namespace Transport.Application.Features.Stops.Commands.UpdateStop
{
    public class UpdateStopCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid SectorId { get; set; }
    }
}
