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
        private readonly ISchoolRepository _schoolRepository;

        public UpdateRouteHandler(IRouteRepository repository, ISchoolRepository schoolRepository)
        {
            _repository = repository;
            _schoolRepository = schoolRepository;
        }

        public async Task<Unit> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.GetByIdWithStopsAsync(request.Id)
                ?? throw new DomainException("Ruta no encontrada");

            if (await _repository.HasActiveTripAsync(route.Id))
                throw new DomainException("No se puede actualizar una ruta con un viaje en progreso");

            if (!await _schoolRepository.ExistsAsync(request.SchoolId))
                throw new DomainException("Escuela no encontrada");

            if (!await _schoolRepository.IsActiveAsync(request.SchoolId))
                throw new DomainException("La escuela está inactiva");

            if (await _repository.ExistsByNameForSchoolAsync(request.Name, request.SchoolId, route.Id))
                throw new DomainException("Ya existe una ruta con ese nombre para la escuela");

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
