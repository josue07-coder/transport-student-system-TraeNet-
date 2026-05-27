using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
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
        private readonly IAuditService _auditService;

        public CreateRouteAssignmentHandler(
            IRouteAssignmentRepository assignmentRepository,
            IRouteRepository routeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            ITransportAssistantRepository transportAssistantRepository,
            IAuditService auditService)
        {
            _assignmentRepository = assignmentRepository;
            _routeRepository = routeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _transportAssistantRepository = transportAssistantRepository;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(CreateRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetByIdWithStopsAsync(request.RouteId)
                ?? throw new DomainException("Ruta no encontrada");

            var driver = await _driverRepository.GetByIdIncludingInactiveAsync(request.DriverId)
                ?? throw new DomainException("Conductor no encontrado");

            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId)
                ?? throw new DomainException("Vehiculo no encontrado");

            if (route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para crear una asignación");

            if (!driver.IsActive)
                throw new DomainException("El conductor está inactivo");

            if (vehicle.Status != VehicleStatus.Active)
                throw new DomainException("El vehiculo está inactivo");

            if (request.TransportAssistantId.HasValue)
            {
                var assistant = await _transportAssistantRepository.GetByIdIncludingInactiveAsync(request.TransportAssistantId.Value)
                    ?? throw new DomainException("Asistente de transporte no encontrado");

                if (!assistant.IsActive)
                    throw new DomainException("El asistente de transporte está inactivo");
            }

            if (request.VehicleCapacity > vehicle.Capacity)
                throw new DomainException("La capacidad de la asignación no puede ser mayor que la capacidad del vehículo");

            if (await _assignmentRepository.HasDriverScheduleConflictAsync(request.DriverId, request.RouteId))
                throw new DomainException("El conductor ya tiene una asignación activa con horario cruzado");

            if (await _assignmentRepository.HasVehicleScheduleConflictAsync(request.VehicleId, request.RouteId))
                throw new DomainException("El vehículo ya tiene una asignación activa con horario cruzado");

            if (request.TransportAssistantId.HasValue &&
                await _assignmentRepository.HasTransportAssistantScheduleConflictAsync(request.TransportAssistantId.Value, request.RouteId))
                throw new DomainException("El asistente de transporte ya tiene una asignación activa con horario cruzado");

            if (request.VehicleCapacity <= 0)
                throw new DomainException("La capacidad del vehículo debe ser mayor que 0");

            var assignment = new RouteAssignment(
                request.RouteId,
                request.VehicleId,
                request.DriverId,
                request.VehicleCapacity,
                request.TransportAssistantId);

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();

            await _auditService.LogAsync("Created", "RouteAssignment", assignment.Id.ToString(), null, $"{{\"RouteId\":\"{assignment.RouteId}\",\"DriverId\":\"{assignment.DriverId}\",\"VehicleId\":\"{assignment.VehicleId}\"}}");

            return assignment.Id;
        }
    }
}
