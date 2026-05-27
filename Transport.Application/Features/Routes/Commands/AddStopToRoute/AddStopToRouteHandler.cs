using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Commands.AddStopToRoute
{
    public class AddStopToRouteHandler : IRequestHandler<AddStopToRouteCommand, Unit>
    {
        private readonly IRouteRepository _repository;
        private readonly IStopRepository _stopRepository;

        public AddStopToRouteHandler(IRouteRepository repository, IStopRepository stopRepository)
        {
            _repository = repository;
            _stopRepository = stopRepository;
        }

        public async Task<Unit> Handle(AddStopToRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.RouteId)
                ?? throw new DomainException("Ruta no encontrada");

            if (await _repository.HasActiveTripAsync(route.Id))
                throw new DomainException("No se pueden agregar paradas a una ruta con un viaje en progreso");

            if (!await _stopRepository.ExistsAsync(request.StopId))
                throw new DomainException("Parada no encontrada");

            var routeStop = new RouteStop(route.Id, request.StopId, request.StopOrder);
            route.AddStop(routeStop);
            _repository.AddRouteStop(routeStop);

            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
