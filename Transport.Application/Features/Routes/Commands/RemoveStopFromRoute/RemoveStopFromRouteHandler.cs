using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Commands.RemoveStopFromRoute
{
    public class RemoveStopFromRouteHandler : IRequestHandler<RemoveStopFromRouteCommand, Unit>
    {
        private readonly IRouteRepository _repository;

        public RemoveStopFromRouteHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveStopFromRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.RouteId)
                ?? throw new DomainException("Ruta no encontrada");

            if (await _repository.HasActiveTripAsync(route.Id))
                throw new DomainException("No se pueden eliminar paradas de una ruta con un viaje en progreso");

            route.RemoveStopByStopId(request.StopId);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
