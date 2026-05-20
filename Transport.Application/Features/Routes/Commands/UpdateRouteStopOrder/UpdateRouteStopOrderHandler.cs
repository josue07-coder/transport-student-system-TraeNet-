using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Routes.Commands.UpdateRouteStopOrder
{
    public class UpdateRouteStopOrderHandler : IRequestHandler<UpdateRouteStopOrderCommand, Unit>
    {
        private readonly IRouteRepository _repository;

        public UpdateRouteStopOrderHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateRouteStopOrderCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.RouteId)
                ?? throw new DomainException("Ruta no encontrada");

            route.UpdateStopOrder(request.StopId, request.StopOrder);
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
