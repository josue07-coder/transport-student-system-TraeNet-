using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Drivers.Commands.DeleteDriver
{
    public class DeleteDriverHandler : IRequestHandler<DeleteDriverCommand, Unit>
    {
        private readonly IDriverRepository _repository;

        public DeleteDriverHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Conductor no encontrado");

            if (await _repository.HasInProgressTripAsync(driver.Id))
                throw new DomainException("No se puede desactivar el conductor porque tiene un viaje en progreso");

            if (await _repository.HasActiveRouteAssignmentAsync(driver.Id))
                throw new DomainException("No se puede desactivar el conductor porque está asignado a una ruta activa");

            driver.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
