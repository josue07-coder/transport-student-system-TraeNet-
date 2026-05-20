using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.UpdateRouteAssignment
{
    public class UpdateRouteAssignmentHandler : IRequestHandler<UpdateRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public UpdateRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IRouteRepository routeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository)
        {
            _assignmentRepository = assignmentRepository;
            _routeRepository = routeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Unit> Handle(UpdateRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (!await _routeRepository.ExistsAsync(request.RouteId))
                throw new DomainException("Ruta no encontrada");

            if (!await _driverRepository.ExistsAsync(request.DriverId))
                throw new DomainException("Conductor no encontrado");

            if (!await _vehicleRepository.ExistsAsync(request.VehicleId))
                throw new DomainException("Vehiculo no encontrado");

            assignment.Update(request.RouteId, request.VehicleId, request.DriverId, request.VehicleCapacity);
            await _assignmentRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
