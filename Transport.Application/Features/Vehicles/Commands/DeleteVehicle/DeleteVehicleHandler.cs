using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, Unit>
    {
        private readonly IVehicleRepository _repository;

        public DeleteVehicleHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Vehiculo no encontrado");

            vehicle.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
