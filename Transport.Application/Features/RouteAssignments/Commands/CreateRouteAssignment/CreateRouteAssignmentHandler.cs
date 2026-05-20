using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.CreateRouteAssignment
{
    public class CreateRouteAssignmentHandler : IRequestHandler<CreateRouteAssignmentCommand, Guid>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITransportAssistantRepository _transportAssistantRepository;

        public CreateRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IRouteRepository routeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            ITransportAssistantRepository transportAssistantRepository)
        {
            _assignmentRepository = assignmentRepository;
            _routeRepository = routeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _transportAssistantRepository = transportAssistantRepository;
        }

        public async Task<Guid> Handle(CreateRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            if (!await _routeRepository.ExistsAsync(request.RouteId))
                throw new DomainException("Ruta no encontrada");

            if (!await _driverRepository.ExistsAsync(request.DriverId))
                throw new DomainException("Conductor no encontrado");

            if (!await _vehicleRepository.ExistsAsync(request.VehicleId))
                throw new DomainException("Vehiculo no encontrado");

            if (request.TransportAssistantId.HasValue && !await _transportAssistantRepository.ExistsAsync(request.TransportAssistantId.Value))
                throw new DomainException("Asistente de transporte no encontrado");

            var assignment = new RouteAssignment(
                request.RouteId,
                request.VehicleId,
                request.DriverId,
                request.VehicleCapacity,
                request.TransportAssistantId);

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();

            return assignment.Id;
        }
    }
}
