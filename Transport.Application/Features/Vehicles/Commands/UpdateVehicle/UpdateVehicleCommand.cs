using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Vehicles.Commands.UpdateVehicle
{
    public class UpdateVehicleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public VehicleStatus Status { get; set; }
    }
}
