using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Vehicles.Commands.UpdateVehicle
{
    public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Unit>
    {
        private readonly IVehicleRepository _repository;

        public UpdateVehicleHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Vehículo no encontrado");

            if (await _repository.ExistsByPlateAsync(request.PlateNumber, vehicle.Id))
                throw new DomainException("Ya existe un vehículo con esta placa");

            if (request.Capacity <= 0)
                throw new DomainException("La capacidad debe ser mayor que 0");

            var maxAssignedStudents = await _repository.GetMaxAssignedStudentCountAsync(vehicle.Id);
            if (request.Capacity < maxAssignedStudents)
                throw new DomainException("La capacidad no puede ser menor que la cantidad de estudiantes asignados en rutas activas");

            var isDisablingVehicle = request.Status == VehicleStatus.Inactive || request.Status == VehicleStatus.Maintenance;
            if (isDisablingVehicle && await _repository.HasInProgressTripAsync(vehicle.Id))
                throw new DomainException("No se puede desactivar o enviar a mantenimiento un vehículo con viaje en progreso");

            if (isDisablingVehicle && await _repository.HasActiveRouteAssignmentAsync(vehicle.Id))
                throw new DomainException("No se puede desactivar o enviar a mantenimiento un vehículo asignado a una ruta activa");

            vehicle.SetPlateNumber(request.PlateNumber);
            vehicle.SetCapacity(request.Capacity);

            if (request.Status == VehicleStatus.Active)
                vehicle.Activate();
            else if (request.Status == VehicleStatus.Inactive)
                vehicle.Deactivate();
            else if (request.Status == VehicleStatus.Maintenance)
                vehicle.SendToMaintenance();

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
