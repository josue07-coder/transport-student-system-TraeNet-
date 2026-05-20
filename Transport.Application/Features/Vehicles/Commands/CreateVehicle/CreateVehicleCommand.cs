using MediatR;

namespace Transport.Application.Features.Vehicles.Commands.CreateVehicle
{
    public class CreateVehicleCommand : IRequest<Guid>
    {
        public string PlateNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
