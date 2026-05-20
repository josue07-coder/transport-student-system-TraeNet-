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
                ?? throw new DomainException("Vehiculo no encontrado");

            vehicle.SetPlateNumber(request.PlateNumber);
            vehicle.SetCapacity(request.Capacity);

            if (request.Status == VehicleStatus.Active)
                vehicle.Activate();
            else if (request.Status == VehicleStatus.Inactive)
                vehicle.Deactivate();
            else if (vehicle.Status != VehicleStatus.Maintenance)
                throw new DomainException("El estado Maintenance aun no tiene una operacion de dominio configurada");

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
