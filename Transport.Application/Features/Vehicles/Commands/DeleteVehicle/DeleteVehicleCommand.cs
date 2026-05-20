using MediatR;

namespace Transport.Application.Features.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
