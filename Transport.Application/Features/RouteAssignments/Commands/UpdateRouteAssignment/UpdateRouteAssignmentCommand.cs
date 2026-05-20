using MediatR;

namespace Transport.Application.Features.RouteAssignments.Commands.UpdateRouteAssignment
{
    public class UpdateRouteAssignmentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public int VehicleCapacity { get; set; }
    }
}
