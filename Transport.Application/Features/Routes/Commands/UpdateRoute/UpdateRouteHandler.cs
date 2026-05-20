using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Routes.Commands.UpdateRoute
{
    public class UpdateRouteHandler : IRequestHandler<UpdateRouteCommand, Unit>
    {
        private readonly IRouteRepository _repository;

        public UpdateRouteHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.Id)
                ?? throw new DomainException("Ruta no encontrada");

            route.SetName(request.Name);
            route.ChangeSchool(request.SchoolId);
            route.UpdateOperatingHours(new TimeRange(request.StartTime, request.EndTime));

            if (request.Status == RouteStatus.Active)
                route.Activate();
            else if (request.Status == RouteStatus.Inactive)
                route.Deactivate();
            else
                throw new DomainException("Solo se permite actualizar la ruta a Active o Inactive.");

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
