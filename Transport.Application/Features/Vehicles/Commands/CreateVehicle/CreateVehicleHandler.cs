using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Vehicles.Commands.CreateVehicle
{
    public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, Guid>
    {
        private readonly IVehicleRepository _repository;

        public CreateVehicleHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByPlateAsync(request.PlateNumber))
                throw new DomainException("Ya existe un vehículo con esta placa");

            var vehicle = new Vehicle(request.PlateNumber, request.Capacity);

            await _repository.AddAsync(vehicle);
            await _repository.SaveChangesAsync();

            return vehicle.Id;
        }
    }
}
