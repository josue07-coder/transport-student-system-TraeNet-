using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, Guid>
    {
        private readonly IRouteRepository _repository;
        private readonly ISchoolRepository _schoolRepository;

        public CreateRouteHandler(IRouteRepository repository, ISchoolRepository schoolRepository)
        {
            _repository = repository;
            _schoolRepository = schoolRepository;
        }

        public async Task<Guid> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            if (!await _schoolRepository.ExistsAsync(request.SchoolId))
                throw new DomainException("Escuela no encontrada");

            if (!await _schoolRepository.IsActiveAsync(request.SchoolId))
                throw new DomainException("La escuela está inactiva");

            if (await _repository.ExistsByNameForSchoolAsync(request.Name, request.SchoolId))
                throw new DomainException("Ya existe una ruta con ese nombre para la escuela");

            var route = new Route(
                request.Name,
                request.SchoolId,
                new TimeRange(request.StartTime, request.EndTime));

            await _repository.AddAsync(route);
            await _repository.SaveChangesAsync();

            return route.Id;
        }
    }
}
