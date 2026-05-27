using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.RouteAssignments.Commands.UpdateRouteAssignment
{
    public class UpdateRouteAssignmentHandler : IRequestHandler<UpdateRouteAssignmentCommand, Unit>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITransportAssistantRepository _transportAssistantRepository;
        private readonly IAuditService _auditService;

        public UpdateRouteAssignmentHandler(
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

        public async Task<Unit> Handle(UpdateRouteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            var oldValues = $"{{\"RouteId\":\"{assignment.RouteId}\",\"DriverId\":\"{assignment.DriverId}\",\"VehicleId\":\"{assignment.VehicleId}\",\"TransportAssistantId\":\"{assignment.TransportAssistantId}\",\"VehicleCapacity\":{assignment.VehicleCapacity}}}";

            if (await _assignmentRepository.HasTripsAsync(assignment.Id))
                throw new DomainException("No se puede actualizar una asignación con viajes asociados");

            var route = await _routeRepository.GetByIdWithStopsAsync(request.RouteId)
                ?? throw new DomainException("Ruta no encontrada");

            var driver = await _driverRepository.GetByIdIncludingInactiveAsync(request.DriverId)
                ?? throw new DomainException("Conductor no encontrado");

            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId)
                ?? throw new DomainException("Vehiculo no encontrado");

            if (route.Status != RouteStatus.Active)
                throw new DomainException("La ruta debe estar activa para actualizar una asignación");

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

            var assignedStudents = await _assignmentRepository.CountStudentsAsync(assignment.Id);
            if (request.VehicleCapacity < assignedStudents)
                throw new DomainException("La nueva capacidad no puede ser menor que la cantidad de estudiantes asignados");

            if (await _assignmentRepository.HasDriverScheduleConflictAsync(request.DriverId, request.RouteId, assignment.Id))
                throw new DomainException("El conductor ya tiene una asignación activa con horario cruzado");

            if (await _assignmentRepository.HasVehicleScheduleConflictAsync(request.VehicleId, request.RouteId, assignment.Id))
                throw new DomainException("El vehículo ya tiene una asignación activa con horario cruzado");

            if (request.TransportAssistantId.HasValue &&
                await _assignmentRepository.HasTransportAssistantScheduleConflictAsync(request.TransportAssistantId.Value, request.RouteId, assignment.Id))
                throw new DomainException("El asistente de transporte ya tiene una asignación activa con horario cruzado");

            assignment.Update(request.RouteId, request.VehicleId, request.DriverId, request.VehicleCapacity, request.TransportAssistantId);
            await _assignmentRepository.SaveChangesAsync();

            var newValues = $"{{\"RouteId\":\"{assignment.RouteId}\",\"DriverId\":\"{assignment.DriverId}\",\"VehicleId\":\"{assignment.VehicleId}\",\"TransportAssistantId\":\"{assignment.TransportAssistantId}\",\"VehicleCapacity\":{assignment.VehicleCapacity}}}";
            await _auditService.LogAsync("Updated", "RouteAssignment", assignment.Id.ToString(), oldValues, newValues);

            return Unit.Value;
        }
    }
}
