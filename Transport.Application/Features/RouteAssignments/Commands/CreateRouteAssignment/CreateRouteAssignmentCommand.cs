using MediatR;

namespace Transport.Application.Features.RouteAssignments.Commands.CreateRouteAssignment
{
    public class CreateRouteAssignmentCommand : IRequest<Guid>
    {
        public Guid RouteId { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public int VehicleCapacity { get; set; }
    }
}
