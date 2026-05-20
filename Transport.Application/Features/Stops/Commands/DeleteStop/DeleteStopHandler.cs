using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Stops.Commands.DeleteStop
{
    public class DeleteStopHandler : IRequestHandler<DeleteStopCommand, Unit>
    {
        private readonly IStopRepository _repository;

        public DeleteStopHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteStopCommand request, CancellationToken cancellationToken)
        {
            var stop = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Parada no encontrada");

            if (await _repository.HasRouteStopsAsync(stop.Id))
                throw new DomainException("No se puede eliminar la parada porque esta asociada a una ruta");

            _repository.Delete(stop);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
