using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Commands.DeleteRoute
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteCommand, Unit>
    {
        private readonly IRouteRepository _repository;

        public DeleteRouteHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Ruta no encontrada");

            if (await _repository.HasActiveTripAsync(route.Id))
                throw new DomainException("No se puede desactivar una ruta con un viaje en progreso");

            route.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
